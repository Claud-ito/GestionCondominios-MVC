using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gestion_Condominios.Migrations
{
    /// <inheritdoc />
    public partial class NombreDireccionEnCondominioNoUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Condominios_Direccion",
                table: "Condominios");

            migrationBuilder.DropIndex(
                name: "IX_Condominios_Nombre",
                table: "Condominios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
