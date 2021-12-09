using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class DamageTypeToClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImmunitiesCache",
                table: "Creatures");

            migrationBuilder.DropColumn(
                name: "ResistancesCahce",
                table: "Creatures");

            migrationBuilder.DropColumn(
                name: "VulnerabilitiesCache",
                table: "Creatures");

            migrationBuilder.AddColumn<string>(
                name: "ImmunityIds",
                table: "Creatures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResistanceIds",
                table: "Creatures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VulnerabilityIds",
                table: "Creatures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DamageTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamageTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DamageTypes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DamageTypes_UserId",
                table: "DamageTypes",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DamageTypes");

            migrationBuilder.DropColumn(
                name: "ImmunityIds",
                table: "Creatures");

            migrationBuilder.DropColumn(
                name: "ResistanceIds",
                table: "Creatures");

            migrationBuilder.DropColumn(
                name: "VulnerabilityIds",
                table: "Creatures");

            migrationBuilder.AddColumn<string>(
                name: "ImmunitiesCache",
                table: "Creatures",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResistancesCahce",
                table: "Creatures",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VulnerabilitiesCache",
                table: "Creatures",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
