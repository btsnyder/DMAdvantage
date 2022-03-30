using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class NotIgnoringDataIntegrity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InitativeData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Initative = table.Column<int>(type: "int", nullable: false),
                    CurrentHP = table.Column<int>(type: "int", nullable: false),
                    CurrentFP = table.Column<int>(type: "int", nullable: false),
                    CurrentTP = table.Column<int>(type: "int", nullable: false),
                    CurrentHitDice = table.Column<int>(type: "int", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatureId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EncounterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InitativeData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InitativeData_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InitativeData_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InitativeData_Creatures_CreatureId",
                        column: x => x.CreatureId,
                        principalTable: "Creatures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InitativeData_Encounters_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "Encounters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InitativeData_CharacterId",
                table: "InitativeData",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_InitativeData_CreatureId",
                table: "InitativeData",
                column: "CreatureId");

            migrationBuilder.CreateIndex(
                name: "IX_InitativeData_EncounterId",
                table: "InitativeData",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_InitativeData_UserId",
                table: "InitativeData",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InitativeData");
        }
    }
}
