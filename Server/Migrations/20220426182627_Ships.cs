using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class Ships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Ships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HullPoints = table.Column<int>(type: "int", nullable: false),
                    HullHitDice = table.Column<int>(type: "int", nullable: false),
                    HullHitDiceNumber = table.Column<int>(type: "int", nullable: false),
                    ShieldPoints = table.Column<int>(type: "int", nullable: false),
                    ShieldHitDice = table.Column<int>(type: "int", nullable: false),
                    ShieldHitDiceNumber = table.Column<int>(type: "int", nullable: false),
                    ArmorClass = table.Column<int>(type: "int", nullable: false),
                    Speed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Strength = table.Column<int>(type: "int", nullable: false),
                    StrengthBonus = table.Column<int>(type: "int", nullable: false),
                    Dexterity = table.Column<int>(type: "int", nullable: false),
                    DexterityBonus = table.Column<int>(type: "int", nullable: false),
                    Constitution = table.Column<int>(type: "int", nullable: false),
                    ConstitutionBonus = table.Column<int>(type: "int", nullable: false),
                    Intelligence = table.Column<int>(type: "int", nullable: false),
                    IntelligenceBonus = table.Column<int>(type: "int", nullable: false),
                    Wisdom = table.Column<int>(type: "int", nullable: false),
                    WisdomBonus = table.Column<int>(type: "int", nullable: false),
                    Charisma = table.Column<int>(type: "int", nullable: false),
                    CharismaBonus = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StrengthSave = table.Column<bool>(type: "bit", nullable: true),
                    DexteritySave = table.Column<bool>(type: "bit", nullable: true),
                    ConstitutionSave = table.Column<bool>(type: "bit", nullable: true),
                    IntelligenceSave = table.Column<bool>(type: "bit", nullable: true),
                    WisdomSave = table.Column<bool>(type: "bit", nullable: true),
                    CharismaSave = table.Column<bool>(type: "bit", nullable: true),
                    Boost = table.Column<bool>(type: "bit", nullable: true),
                    Ram = table.Column<bool>(type: "bit", nullable: true),
                    Hide = table.Column<bool>(type: "bit", nullable: true),
                    Maneuvering = table.Column<bool>(type: "bit", nullable: true),
                    Patch = table.Column<bool>(type: "bit", nullable: true),
                    Regulation = table.Column<bool>(type: "bit", nullable: true),
                    Astrogation = table.Column<bool>(type: "bit", nullable: true),
                    Data = table.Column<bool>(type: "bit", nullable: true),
                    Scan = table.Column<bool>(type: "bit", nullable: true),
                    Impress = table.Column<bool>(type: "bit", nullable: true),
                    Interfere = table.Column<bool>(type: "bit", nullable: true),
                    Menace = table.Column<bool>(type: "bit", nullable: true),
                    Swindle = table.Column<bool>(type: "bit", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ships_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

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
                name: "IX_Ships_UserId",
                table: "Ships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipWeapon_WeaponsId",
                table: "ShipWeapon",
                column: "WeaponsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipWeapon");

            migrationBuilder.DropTable(
                name: "Ships");

            migrationBuilder.DropColumn(
                name: "Ship",
                table: "Weapons");

            migrationBuilder.DropColumn(
                name: "Ship",
                table: "WeaponProperties");
        }
    }
}
