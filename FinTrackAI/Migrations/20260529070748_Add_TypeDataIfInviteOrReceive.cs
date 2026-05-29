using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinTrackAI.Migrations
{
    /// <inheritdoc />
    public partial class Add_TypeDataIfInviteOrReceive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CPF",
                table: "Transacao",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ChavePix_ALVO",
                table: "Transacao",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Nome_Destino",
                table: "Transacao",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Nome_Origem",
                table: "Transacao",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Recebeu_Enviou",
                table: "Transacao",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CPF",
                table: "Transacao");

            migrationBuilder.DropColumn(
                name: "ChavePix_ALVO",
                table: "Transacao");

            migrationBuilder.DropColumn(
                name: "Nome_Destino",
                table: "Transacao");

            migrationBuilder.DropColumn(
                name: "Nome_Origem",
                table: "Transacao");

            migrationBuilder.DropColumn(
                name: "Recebeu_Enviou",
                table: "Transacao");
        }
    }
}
