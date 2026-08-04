using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiUsuarios.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenToUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenCriadoEm",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiracao",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshTokenHash",
                table: "Usuarios",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenRevogadoEm",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshTokenCriadoEm",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiracao",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RefreshTokenHash",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RefreshTokenRevogadoEm",
                table: "Usuarios");
        }
    }
}
