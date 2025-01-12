using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace latexapi.Migrations
{
    /// <inheritdoc />
    public partial class Update1206 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfFile",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "IsCompile",
                table: "Files");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Versions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MainFileId",
                table: "Versions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "PdfFile",
                table: "Versions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPublic",
                table: "Projects",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainFileId",
                table: "Versions");

            migrationBuilder.DropColumn(
                name: "PdfFile",
                table: "Versions");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Versions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPublic",
                table: "Projects",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PdfFile",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompile",
                table: "Files",
                type: "bit",
                nullable: true);
        }
    }
}
