# Sistema de Gestión de Condominios

Aplicación web de gestión de condominios construida con **ASP.NET Core MVC (C#)** y **Entity Framework Core**. Permite administrar condominios, casas, personas residentes, expensas mensuales, visitantes y registros de visitas, y consultar reportes con datos reales de la base de datos.

Proyecto académico grupal de Programación Web 3.

## Integrantes

| Integrante | Usuario de GitHub | Módulo |
|---|---|---|
| Said Montaño | [Claud-ito](https://github.com/Claud-ito) | Condominios |
| Mateo Salazar | _(pendiente)_ | Casas, personas y ocupación |
| Matias Raldes | _(pendiente)_ | Expensas y pagos |
| Naim | _(pendiente)_ | Visitantes y visitas |
| Sebastian Eguez | [sebaxtianmur89](https://github.com/sebaxtianmur89) | Integración, interfaz compartida y documentación |

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (por ejemplo SQL Server Express o LocalDB) en ejecución
- Herramienta de migraciones de EF Core:

  ```bash
  dotnet tool install --global dotnet-ef
  ```

- Opcional: Visual Studio o VS Code

## Configuración de la base de datos

1. Revisar la cadena de conexión `GestionCondominio` en `Gestion-Condominios/appsettings.json`. Por defecto usa autenticación de Windows y no contiene contraseñas:

   ```
   Server=localhost; Database=DBGestCondominios; Trusted_Connection=True; TrustServerCertificate=True
   ```

   Si tu SQL Server tiene otro nombre de instancia, cámbialo en tu copia local sin subir credenciales reales al repositorio.

2. Crear la base de datos aplicando las migraciones, desde la raíz del repositorio:

   ```bash
   dotnet ef database update --project Gestion-Condominios
   ```

   Esto crea la base `DBGestCondominios` con las siete tablas del modelo: `Condominio`, `Unidad`, `Persona`, `UnidadPersona`, `Pago`, `Visitante` y `Visita`.

## Ejecución

```bash
dotnet run --project Gestion-Condominios
```

Luego abrir en el navegador la dirección HTTPS que indique la consola. También se puede abrir `Gestion-Condominios.slnx` en Visual Studio y ejecutar con F5.

## Reportes

> Los reportes están en desarrollo. Esta sección se actualizará con los filtros definitivos cuando se integren.

1. **Expensas y cobranza por período.** Muestra cuánto se facturó y cuánto se cobró en cada período, e identifica expensas pendientes o vencidas. Permite filtrar por condominio, casa, año, mes y estado de pago, y resume total facturado, total cobrado y total pendiente.
2. **Bitácora de visitas.** Lista los ingresos y salidas de visitantes y permite detectar visitas sin hora de salida registrada. Permite filtrar por rango de fechas, condominio, casa y visitante, y muestra la cantidad total de visitas y las abiertas.

## Estructura del proyecto

```
Gestion-Condominios/
├── Controllers/     # Controladores MVC
├── Data/            # DbContext y configuraciones Fluent API
├── Migrations/      # Migraciones de EF Core
├── Models/          # Entidades del dominio
├── ViewModels/      # ViewModels de formularios y reportes
├── Views/           # Vistas Razor
└── wwwroot/         # Archivos estáticos (CSS, JS, librerías)
```

## Flujo de trabajo en GitHub

Cada integrante trabaja en una rama propia y no en `main`. Al terminar su módulo sube la rama con `git push -u origin nombre-de-la-rama` y la une a `main` mediante un Pull Request.
