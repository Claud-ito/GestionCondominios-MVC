using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gestion_Condominios.Migrations
{
    /// <inheritdoc />
    public partial class migracioInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Condominios",
                columns: table => new
                {
                    IdCondominio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Condominios", x => x.IdCondominio);
                    table.CheckConstraint("CK_Condominios_Direccion", "LEN(LTRIM(RTRIM([Direccion]))) > 0");
                    table.CheckConstraint("CK_Condominios_Nombre", "LEN(LTRIM(RTRIM([Nombre]))) > 0");
                });

            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    IdPersona = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NroCI = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    Correo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.IdPersona);
                    table.CheckConstraint("CK_Personas_Apellido", "LEN(LTRIM(RTRIM([Apellido]))) > 0");
                    table.CheckConstraint("CK_Personas_Nombre", "LEN(LTRIM(RTRIM([Nombre]))) > 0");
                    table.CheckConstraint("CK_Personas_NroCI", "LEN(LTRIM(RTRIM([NroCI]))) > 0");
                });

            migrationBuilder.CreateTable(
                name: "Visitantes",
                columns: table => new
                {
                    IdVisitante = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NroCI = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitantes", x => x.IdVisitante);
                    table.CheckConstraint("CK_Visitantes_Apellido", "LEN(LTRIM(RTRIM([Apellido]))) > 0");
                    table.CheckConstraint("CK_Visitantes_Nombre", "LEN(LTRIM(RTRIM([Nombre]))) > 0");
                    table.CheckConstraint("CK_Visitantes_NroCI", "LEN(LTRIM(RTRIM([NroCI]))) > 0");
                });

            migrationBuilder.CreateTable(
                name: "Unidades",
                columns: table => new
                {
                    IdUnidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCondominio = table.Column<int>(type: "int", nullable: false),
                    NumeroUnidad = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CantidadCuartos = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SuperficieM2 = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    PagoMes = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unidades", x => x.IdUnidad);
                    table.CheckConstraint("CK_Unidades_CantidadCuartos", "[CantidadCuartos] >= 0");
                    table.CheckConstraint("CK_Unidades_Numero", "LEN(LTRIM(RTRIM([NumeroUnidad]))) > 0");
                    table.CheckConstraint("CK_Unidades_PagoMes", "[PagoMes] > 0");
                    table.CheckConstraint("CK_Unidades_Superficie", "[SuperficieM2] > 0");
                    table.CheckConstraint("CK_Unidades_Tipo", "LEN(LTRIM(RTRIM([Tipo]))) > 0");
                    table.ForeignKey(
                        name: "FK_Unidades_Condominios_IdCondominio",
                        column: x => x.IdCondominio,
                        principalTable: "Condominios",
                        principalColumn: "IdCondominio");
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    IdPago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "date", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "date", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false, defaultValue: "PENDIENTE"),
                    Concepto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    IdUnidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.IdPago);
                    table.CheckConstraint("CK_Pagos_Anio", "[Anio] BETWEEN 1 AND 9999");
                    table.CheckConstraint("CK_Pagos_Concepto", "LEN(LTRIM(RTRIM([Concepto]))) > 0");
                    table.CheckConstraint("CK_Pagos_EstadoFecha", "(\r\n                    ([Estado] = 'PENDIENTE'\r\n                     AND [FechaPago] IS NULL)\r\n                    OR\r\n                    ([Estado] = 'PAGADO'\r\n                     AND [FechaPago] IS NOT NULL)\r\n                  )");
                    table.CheckConstraint("CK_Pagos_Mes", "[Mes] BETWEEN 1 AND 12");
                    table.CheckConstraint("CK_Pagos_Monto", "[Monto] > 0");
                    table.ForeignKey(
                        name: "FK_Pagos_Unidades_IdUnidad",
                        column: x => x.IdUnidad,
                        principalTable: "Unidades",
                        principalColumn: "IdUnidad");
                });

            migrationBuilder.CreateTable(
                name: "UnidadPersonas",
                columns: table => new
                {
                    IdUnidad = table.Column<int>(type: "int", nullable: false),
                    IdPersona = table.Column<int>(type: "int", nullable: false),
                    TipoRelacion = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadPersonas", x => new { x.IdUnidad, x.IdPersona });
                    table.CheckConstraint("CK_UnidadPersonas_TipoRelacion", "[TipoRelacion] IN ('INQUILINO', 'HABITANTE')");
                    table.ForeignKey(
                        name: "FK_UnidadPersonas_Personas_IdPersona",
                        column: x => x.IdPersona,
                        principalTable: "Personas",
                        principalColumn: "IdPersona");
                    table.ForeignKey(
                        name: "FK_UnidadPersonas_Unidades_IdUnidad",
                        column: x => x.IdUnidad,
                        principalTable: "Unidades",
                        principalColumn: "IdUnidad");
                });

            migrationBuilder.CreateTable(
                name: "Visitas",
                columns: table => new
                {
                    IdVisita = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdVisitante = table.Column<int>(type: "int", nullable: false),
                    IdUnidad = table.Column<int>(type: "int", nullable: false),
                    FechaHoraIngreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaHoraSalida = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitas", x => x.IdVisita);
                    table.CheckConstraint("CK_Visitas_Fechas", "[FechaHoraSalida] IS NULL\r\n                  OR [FechaHoraSalida] >= [FechaHoraIngreso]");
                    table.ForeignKey(
                        name: "FK_Visitas_Unidades_IdUnidad",
                        column: x => x.IdUnidad,
                        principalTable: "Unidades",
                        principalColumn: "IdUnidad");
                    table.ForeignKey(
                        name: "FK_Visitas_Visitantes_IdVisitante",
                        column: x => x.IdVisitante,
                        principalTable: "Visitantes",
                        principalColumn: "IdVisitante");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Condominios_Direccion",
                table: "Condominios",
                column: "Direccion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Condominios_Nombre",
                table: "Condominios",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Pagos_Unidad_Anio_Mes",
                table: "Pagos",
                columns: new[] { "IdUnidad", "Anio", "Mes" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Personas_NroCI",
                table: "Personas",
                column: "NroCI",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Unidades_Condominio_Numero",
                table: "Unidades",
                columns: new[] { "IdCondominio", "NumeroUnidad" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnidadPersonas_IdPersona",
                table: "UnidadPersonas",
                column: "IdPersona");

            migrationBuilder.CreateIndex(
                name: "UX_UnidadPersonas_Inquilino_Unidad",
                table: "UnidadPersonas",
                column: "IdUnidad",
                unique: true,
                filter: "[TipoRelacion] = 'INQUILINO'");

            migrationBuilder.CreateIndex(
                name: "UX_Visitantes_NroCI",
                table: "Visitantes",
                column: "NroCI",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visitas_Unidad_FechaIngreso",
                table: "Visitas",
                columns: new[] { "IdUnidad", "FechaHoraIngreso" });

            migrationBuilder.CreateIndex(
                name: "IX_Visitas_Visitante_FechaIngreso",
                table: "Visitas",
                columns: new[] { "IdVisitante", "FechaHoraIngreso" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "UnidadPersonas");

            migrationBuilder.DropTable(
                name: "Visitas");

            migrationBuilder.DropTable(
                name: "Personas");

            migrationBuilder.DropTable(
                name: "Unidades");

            migrationBuilder.DropTable(
                name: "Visitantes");

            migrationBuilder.DropTable(
                name: "Condominios");
        }
    }
}
