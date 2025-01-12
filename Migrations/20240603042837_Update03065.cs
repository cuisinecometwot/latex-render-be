using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace latexapi.Migrations
{
    /// <inheritdoc />
    public partial class Update03065 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_LastestVersionId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_LastestVersionId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "LastestVersionId",
                table: "Projects");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_LastModifiedUserId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_LastModifiedUserId",
                table: "Projects");

            migrationBuilder.AddColumn<Guid>(
                name: "LastestVersionId",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LastestVersionId",
                table: "Projects",
                column: "LastestVersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_LastestVersionId",
                table: "Projects",
                column: "LastestVersionId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
