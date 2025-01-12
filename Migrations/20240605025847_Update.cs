using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace latexapi.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_LastModifiedUserId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_LastModifiedUserId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "LastestVersionId",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "LastModifiedUserId",
                table: "Projects",
                newName: "MainVersionId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedTime",
                table: "Versions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MainVersionId",
                table: "Projects",
                newName: "LastModifiedUserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedTime",
                table: "Versions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastestVersionId",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LastModifiedUserId",
                table: "Projects",
                column: "LastModifiedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_LastModifiedUserId",
                table: "Projects",
                column: "LastModifiedUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
