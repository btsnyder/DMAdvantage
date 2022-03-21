using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class ForcePowerFromId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForcePowerIds",
                table: "Creatures");

            migrationBuilder.DropColumn(
                name: "ForcePowerIds",
                table: "Characters");

            migrationBuilder.CreateTable(
                name: "CharacterForcePower",
                columns: table => new
                {
                    CharactersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ForcePowersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterForcePower", x => new { x.CharactersId, x.ForcePowersId });
                    table.ForeignKey(
                        name: "FK_CharacterForcePower_Characters_CharactersId",
                        column: x => x.CharactersId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterForcePower_ForcePowers_ForcePowersId",
                        column: x => x.ForcePowersId,
                        principalTable: "ForcePowers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreatureForcePower",
                columns: table => new
                {
                    CreaturesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ForcePowersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatureForcePower", x => new { x.CreaturesId, x.ForcePowersId });
                    table.ForeignKey(
                        name: "FK_CreatureForcePower_Creatures_CreaturesId",
                        column: x => x.CreaturesId,
                        principalTable: "Creatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreatureForcePower_ForcePowers_ForcePowersId",
                        column: x => x.ForcePowersId,
                        principalTable: "ForcePowers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterForcePower_ForcePowersId",
                table: "CharacterForcePower",
                column: "ForcePowersId");

            migrationBuilder.CreateIndex(
                name: "IX_CreatureForcePower_ForcePowersId",
                table: "CreatureForcePower",
                column: "ForcePowersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterForcePower");

            migrationBuilder.DropTable(
                name: "CreatureForcePower");

            migrationBuilder.AddColumn<string>(
                name: "ForcePowerIds",
                table: "Creatures",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForcePowerIds",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
