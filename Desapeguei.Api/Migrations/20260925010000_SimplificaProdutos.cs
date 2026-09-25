using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desapeguei.Api.Migrations
{
    /// <inheritdoc />
    public partial class SimplificaProdutos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagemUrl",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "CriadoPor",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "CaminhoVideo",
                table: "Produtos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagemUrl",
                table: "Produtos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CriadoPor",
                table: "Produtos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CaminhoVideo",
                table: "Produtos",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
