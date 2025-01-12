using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace latexapi.Migrations
{
    /// <inheritdoc />
    public partial class Update03067 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LastestVersionId",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastestVersionId",
                table: "Projects");
        }
    }
}
