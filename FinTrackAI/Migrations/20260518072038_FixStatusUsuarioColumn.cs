using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinTrackAI.Migrations
{
    /// <inheritdoc />
    public partial class FixStatusUsuarioColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StatusUsuario",
                table: "Users",
                type: "longtext",
                nullable: false,
                defaultValue: "ATIVO",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StatusUsuario",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldDefaultValue: "ATIVO")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
