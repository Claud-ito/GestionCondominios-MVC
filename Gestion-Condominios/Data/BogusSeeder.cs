using Bogus;
using Gestion_Condominios.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;

namespace Gestion_Condominios.Data;

/// <summary>Datos ficticios para una base dedicada a pruebas y vacía.</summary>
public static class BogusSeeder
{
    public static async Task CargarAsync(CondominioDbContext db)
    {
        // La transacción incluye la comprobación y toda la carga de datos.
        await using var transaccion = await db.Database
            .BeginTransactionAsync(IsolationLevel.Serializable);

        if (await db.Condominios.AnyAsync() || await db.Unidades.AnyAsync()
            || await db.Personas.AnyAsync() || await db.UnidadesPersonas.AnyAsync()
            || await db.Pagos.AnyAsync() || await db.Visitantes.AnyAsync()
            || await db.Visitas.AnyAsync())
        {
            Console.WriteLine("Carga omitida: ya existen datos. No se modificó ningún registro.");
            return;
        }

        // Semillas locales: no afectan a otros usos de Bogus en la aplicación.
        var faker = new Faker("es") { Random = new Randomizer(20261027) };
        var ahora = DateTime.Now;
        var mesActual = new DateTime(ahora.Year, ahora.Month, 1);
        var inicioOcupacion = mesActual.AddMonths(-6);

        var nombres = new[]
        {
            "DEMO Los Pinos", "DEMO Las Palmeras", "DEMO Mirador",
            "DEMO Sin unidades"
        };
        var condominios = nombres.Select((nombre, i) => new Condominio
        {
            Nombre = nombre,
            Direccion = $"Calle de Prueba {i + 1}, zona DEMO, Cochabamba",
            Telefono = faker.Random.Number(60000000, 79999999)
                .ToString(CultureInfo.InvariantCulture)
        }).ToList();

        var unidades = new List<Unidad>();
        var ocupadas = new List<Unidad>();
        // 3 condominios con 10 unidades cada uno; el cuarto queda vacío.
        foreach (var condominio in condominios.Take(3))
        {
            for (int numero = 1; numero <= 10; numero++)
            {
                var unidad = new Unidad
                {
                    Condominio = condominio,
                    NumeroUnidad = $"U-{numero:D2}",
                    CantidadCuartos = faker.Random.Number(1, 4),
                    Tipo = numero % 2 == 0 ? "DEPARTAMENTO" : "CASA",
                    SuperficieM2 = faker.Random.Number(50, 180),
                    PagoMes = faker.Random.Number(3, 9) * 100m
                };
                unidades.Add(unidad);
                if (numero <= 8) ocupadas.Add(unidad);
            }
        }

        int secuenciaPersona = 0;
        var personas = new Faker<Persona>("es")
            .UseSeed(20261028)
            .RuleFor(p => p.NroCI, _ => (90000000 + ++secuenciaPersona)
                .ToString(CultureInfo.InvariantCulture))
            .RuleFor(p => p.Nombre, f => Limitar(f.Name.FirstName(), 25))
            .RuleFor(p => p.Apellido, f => Limitar(f.Name.LastName(), 40))
            .RuleFor(p => p.Telefono, f => f.Random.Number(60000000, 79999999)
                .ToString(CultureInfo.InvariantCulture))
            .RuleFor(p => p.Correo, (_, p) => $"persona{p.NroCI}@example.com")
            .Generate(60);

        var relaciones = new List<UnidadPersona>();
        for (int i = 0; i < ocupadas.Count; i++)
        {
            relaciones.Add(new UnidadPersona
            {
                Unidad = ocupadas[i], Persona = personas[i],
                TipoRelacion = "INQUILINO", FechaInicio = inicioOcupacion
            });
            relaciones.Add(new UnidadPersona
            {
                Unidad = ocupadas[i], Persona = personas[24 + i],
                TipoRelacion = "HABITANTE", FechaInicio = inicioOcupacion.AddDays(7)
            });
        }
        // Una persona puede estar vinculada a varias unidades.
        relaciones.Add(new UnidadPersona
        {
            Unidad = ocupadas[1], Persona = personas[0],
            TipoRelacion = "HABITANTE", FechaInicio = inicioOcupacion.AddDays(10)
        });

        var pagos = new List<Pago>();
        for (int i = 0; i < ocupadas.Count; i++)
        {
            var unidad = ocupadas[i];
            // Solo meses anteriores: el mes actual se generará desde la interfaz.
            for (int mesesAtras = 3; mesesAtras >= 1; mesesAtras--)
            {
                var periodo = mesActual.AddMonths(-mesesAtras);
                bool pagado = i % 3 == 0
                    || (i % 3 == 1 && mesesAtras >= 2)
                    || (i % 3 == 2 && mesesAtras == 3);

                pagos.Add(new Pago
                {
                    Unidad = unidad,
                    Monto = unidad.PagoMes,
                    Anio = periodo.Year,
                    Mes = periodo.Month,
                    Concepto = $"Expensa DEMO {periodo:MM/yyyy}",
                    FechaVencimiento = periodo.AddMonths(1).AddDays(-1),
                    Estado = pagado ? "PAGADO" : "PENDIENTE",
                    FechaPago = pagado ? periodo.AddDays(9) : (DateTime?)null
                });
            }
        }

        int secuenciaVisitante = 0;
        var visitantes = new Faker<Visitante>("es")
            .UseSeed(20261029)
            .RuleFor(v => v.NroCI, _ => (80000000 + ++secuenciaVisitante)
                .ToString(CultureInfo.InvariantCulture))
            .RuleFor(v => v.Nombre, f => Limitar(f.Name.FirstName(), 50))
            .RuleFor(v => v.Apellido, f => Limitar(f.Name.LastName(), 100))
            .Generate(40);

        var visitas = new List<Visita>();
        for (int i = 0; i < visitantes.Count; i++)
        {
            // Dos visitas terminadas por visitante, en días diferentes.
            for (int semana = 1; semana <= 2; semana++)
            {
                var ingreso = ahora.Date.AddDays(-7 * semana)
                    .AddHours(9 + i % 8).AddMinutes(faker.Random.Number(0, 30));
                visitas.Add(new Visita
                {
                    Visitante = visitantes[i],
                    Unidad = ocupadas[i % ocupadas.Count],
                    FechaHoraIngreso = ingreso,
                    FechaHoraSalida = ingreso.AddMinutes(faker.Random.Number(30, 180))
                });
            }
        }
        // Ocho visitantes distintos permanecen dentro: permite probar Registrar salida.
        for (int i = 0; i < 8; i++)
        {
            visitas.Add(new Visita
            {
                Visitante = visitantes[i], Unidad = ocupadas[i],
                FechaHoraIngreso = ahora.AddMinutes(-5 * (i + 1)),
                FechaHoraSalida = null
            });
        }

        // Se relacionan objetos, no IDs inventados: EF obtiene los IDs de SQL Server.
        db.Condominios.AddRange(condominios);
        db.Unidades.AddRange(unidades);
        db.Personas.AddRange(personas);
        db.UnidadesPersonas.AddRange(relaciones);
        db.Pagos.AddRange(pagos);
        db.Visitantes.AddRange(visitantes);
        db.Visitas.AddRange(visitas);

        await db.SaveChangesAsync();
        await transaccion.CommitAsync();

        Console.WriteLine("Datos DEMO cargados correctamente:");
        Console.WriteLine($"Condominios: {condominios.Count}; unidades: {unidades.Count}; ocupadas: {ocupadas.Count}.");
        Console.WriteLine($"Personas: {personas.Count}; relaciones de ocupación: {relaciones.Count}.");
        Console.WriteLine($"Expensas: {pagos.Count}; pagadas: {pagos.Count(p => p.Estado == "PAGADO")}; pendientes: {pagos.Count(p => p.Estado == "PENDIENTE")}.");
        Console.WriteLine($"Visitantes: {visitantes.Count}; visitas: {visitas.Count}; abiertas: {visitas.Count(v => v.FechaHoraSalida == null)}.");
        Console.WriteLine("Ahora inicie la aplicación sin --SeedDemo=true.");
    }

    private static string Limitar(string texto, int longitud)
    {
        texto = texto.Trim();
        return texto.Length <= longitud ? texto : texto[..longitud].TrimEnd();
    }
}
