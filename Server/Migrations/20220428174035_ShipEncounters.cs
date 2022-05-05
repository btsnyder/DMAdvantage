using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class ShipEncounters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ships_AspNetUsers_UserId",
                table: "Ships");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipShipWeapon_Ships_ShipsId",
                table: "ShipShipWeapon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ships",
                table: "Ships");

            migrationBuilder.RenameTable(
                name: "Ships",
                newName: "EnemyShips");

            migrationBuilder.RenameIndex(
                name: "IX_Ships_UserId",
                table: "EnemyShips",
                newName: "IX_EnemyShips_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EnemyShips",
                table: "EnemyShips",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ShipEncounters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentPlayer = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipEncounters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipEncounters_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ShipInitativeData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Initative = table.Column<int>(type: "int", nullable: false),
                    CurrentHull = table.Column<int>(type: "int", nullable: false),
                    CurrentShield = table.Column<int>(type: "int", nullable: false),
                    CurrentHullHitDice = table.Column<int>(type: "int", nullable: false),
                    CurrentShieldHitDice = table.Column<int>(type: "int", nullable: false),
                    PlayerShipId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EnemyShipId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShipEncounterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipInitativeData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipInitativeData_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShipInitativeData_EnemyShips_EnemyShipId",
                        column: x => x.EnemyShipId,
                        principalTable: "EnemyShips",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShipInitativeData_EnemyShips_PlayerShipId",
                        column: x => x.PlayerShipId,
                        principalTable: "EnemyShips",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShipInitativeData_ShipEncounters_ShipEncounterId",
                        column: x => x.ShipEncounterId,
                        principalTable: "ShipEncounters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipEncounters_UserId",
                table: "ShipEncounters",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipInitativeData_EnemyShipId",
                table: "ShipInitativeData",
                column: "EnemyShipId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipInitativeData_PlayerShipId",
                table: "ShipInitativeData",
                column: "PlayerShipId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipInitativeData_ShipEncounterId",
                table: "ShipInitativeData",
                column: "ShipEncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipInitativeData_UserId",
                table: "ShipInitativeData",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EnemyShips_AspNetUsers_UserId",
                table: "EnemyShips",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipShipWeapon_EnemyShips_ShipsId",
                table: "ShipShipWeapon",
                column: "ShipsId",
                principalTable: "EnemyShips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnemyShips_AspNetUsers_UserId",
                table: "EnemyShips");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipShipWeapon_EnemyShips_ShipsId",
                table: "ShipShipWeapon");

            migrationBuilder.DropTable(
                name: "ShipInitativeData");

            migrationBuilder.DropTable(
                name: "ShipEncounters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EnemyShips",
                table: "EnemyShips");

            migrationBuilder.RenameTable(
                name: "EnemyShips",
                newName: "Ships");

            migrationBuilder.RenameIndex(
                name: "IX_EnemyShips_UserId",
                table: "Ships",
                newName: "IX_Ships_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ships",
                table: "Ships",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ships_AspNetUsers_UserId",
                table: "Ships",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipShipWeapon_Ships_ShipsId",
                table: "ShipShipWeapon",
                column: "ShipsId",
                principalTable: "Ships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
