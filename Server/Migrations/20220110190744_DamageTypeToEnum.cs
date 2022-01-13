using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class DamageTypeToEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DamageTypes");

            migrationBuilder.RenameColumn(
                name: "VulnerabilityIds",
                table: "Creatures",
                newName: "Vulnerabilities");

            migrationBuilder.RenameColumn(
                name: "ResistanceIds",
                table: "Creatures",
                newName: "Resistances");

            migrationBuilder.RenameColumn(
                name: "ImmunityIds",
                table: "Creatures",
                newName: "Immunities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Vulnerabilities",
                table: "Creatures",
                newName: "VulnerabilityIds");

            migrationBuilder.RenameColumn(
                name: "Resistances",
                table: "Creatures",
                newName: "ResistanceIds");

            migrationBuilder.RenameColumn(
                name: "Immunities",
                table: "Creatures",
                newName: "ImmunityIds");

            migrationBuilder.CreateTable(
                name: "DamageTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
    }
}
