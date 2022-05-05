using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class ShipWeapons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipWeapon");

            migrationBuilder.DropColumn(
                name: "Ship",
                table: "Weapons");

            migrationBuilder.DropColumn(
                name: "Ship",
                table: "WeaponProperties");

            migrationBuilder.CreateTable(
                name: "ShipWeaponProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipWeaponProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipWeaponProperties_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ShipWeapons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Damage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DamageType = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipWeapons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipWeapons_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ShipShipWeapon",
                columns: table => new
                {
                    ShipsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WeaponsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipShipWeapon", x => new { x.ShipsId, x.WeaponsId });
                    table.ForeignKey(
                        name: "FK_ShipShipWeapon_Ships_ShipsId",
                        column: x => x.ShipsId,
                        principalTable: "Ships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipShipWeapon_ShipWeapons_WeaponsId",
                        column: x => x.WeaponsId,
                        principalTable: "ShipWeapons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipWeaponShipWeaponProperty",
                columns: table => new
                {
                    PropertiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WeaponsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipWeaponShipWeaponProperty", x => new { x.PropertiesId, x.WeaponsId });
                    table.ForeignKey(
                        name: "FK_ShipWeaponShipWeaponProperty_ShipWeaponProperties_PropertiesId",
                        column: x => x.PropertiesId,
                        principalTable: "ShipWeaponProperties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipWeaponShipWeaponProperty_ShipWeapons_WeaponsId",
                        column: x => x.WeaponsId,
                        principalTable: "ShipWeapons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipShipWeapon_WeaponsId",
                table: "ShipShipWeapon",
                column: "WeaponsId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipWeaponProperties_UserId",
                table: "ShipWeaponProperties",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipWeapons_UserId",
                table: "ShipWeapons",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipWeaponShipWeaponProperty_WeaponsId",
                table: "ShipWeaponShipWeaponProperty",
                column: "WeaponsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipShipWeapon");

            migrationBuilder.DropTable(
                name: "ShipWeaponShipWeaponProperty");

            migrationBuilder.DropTable(
                name: "ShipWeaponProperties");

            migrationBuilder.DropTable(
                name: "ShipWeapons");

            migrationBuilder.AddColumn<bool>(
                name: "Ship",
                table: "Weapons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Ship",
                table: "WeaponProperties",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ShipWeapon",
                columns: table => new
                {
                    ShipsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WeaponsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipWeapon", x => new { x.ShipsId, x.WeaponsId });
                    table.ForeignKey(
                        name: "FK_ShipWeapon_Ships_ShipsId",
                        column: x => x.ShipsId,
                        principalTable: "Ships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipWeapon_Weapons_WeaponsId",
                        column: x => x.WeaponsId,
                        principalTable: "Weapons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipWeapon_WeaponsId",
                table: "ShipWeapon",
                column: "WeaponsId");
        }
    }
}
