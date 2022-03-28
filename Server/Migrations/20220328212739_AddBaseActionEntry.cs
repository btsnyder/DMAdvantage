using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class AddBaseActionEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionsCache",
                table: "Creatures");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatureId",
                table: "Weapons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Actions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Range = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Damage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DamageType = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BaseActionCreature",
                columns: table => new
                {
                    ActionsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreaturesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseActionCreature", x => new { x.ActionsId, x.CreaturesId });
                    table.ForeignKey(
                        name: "FK_BaseActionCreature_Actions_ActionsId",
                        column: x => x.ActionsId,
                        principalTable: "Actions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaseActionCreature_Creatures_CreaturesId",
                        column: x => x.CreaturesId,
                        principalTable: "Creatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Weapons_CreatureId",
                table: "Weapons",
                column: "CreatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_UserId",
                table: "Actions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseActionCreature_CreaturesId",
                table: "BaseActionCreature",
                column: "CreaturesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Weapons_Creatures_CreatureId",
                table: "Weapons",
                column: "CreatureId",
                principalTable: "Creatures",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Weapons_Creatures_CreatureId",
                table: "Weapons");

            migrationBuilder.DropTable(
                name: "BaseActionCreature");

            migrationBuilder.DropTable(
                name: "Actions");

            migrationBuilder.DropIndex(
                name: "IX_Weapons_CreatureId",
                table: "Weapons");

            migrationBuilder.DropColumn(
                name: "CreatureId",
                table: "Weapons");

            migrationBuilder.AddColumn<string>(
                name: "ActionsCache",
                table: "Creatures",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
