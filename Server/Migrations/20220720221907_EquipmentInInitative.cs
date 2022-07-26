using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class EquipmentInInitative : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterEquipment_Characters_CharacterId",
                table: "CharacterEquipment");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterEquipment_Equipment_EquipmentId",
                table: "CharacterEquipment");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_AspNetUsers_UserId",
                table: "Equipment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Equipment",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "CharacterEquipment");

            migrationBuilder.RenameTable(
                name: "Equipment",
                newName: "Equipments");

            migrationBuilder.RenameColumn(
                name: "EquipmentId",
                table: "CharacterEquipment",
                newName: "EquipmentsId");

            migrationBuilder.RenameColumn(
                name: "CharacterId",
                table: "CharacterEquipment",
                newName: "CharactersId");

            migrationBuilder.RenameIndex(
                name: "IX_CharacterEquipment_EquipmentId",
                table: "CharacterEquipment",
                newName: "IX_CharacterEquipment_EquipmentsId");

            migrationBuilder.RenameIndex(
                name: "IX_Equipment_UserId",
                table: "Equipments",
                newName: "IX_Equipments_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Equipments",
                table: "Equipments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "InitativeEquipmentQuantity",
                columns: table => new
                {
                    InitativeDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InitativeEquipmentQuantity", x => new { x.InitativeDataId, x.EquipmentId });
                    table.ForeignKey(
                        name: "FK_InitativeEquipmentQuantity_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InitativeEquipmentQuantity_InitativeData_InitativeDataId",
                        column: x => x.InitativeDataId,
                        principalTable: "InitativeData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InitativeEquipmentQuantity_EquipmentId",
                table: "InitativeEquipmentQuantity",
                column: "EquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterEquipment_Characters_CharactersId",
                table: "CharacterEquipment",
                column: "CharactersId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterEquipment_Equipments_EquipmentsId",
                table: "CharacterEquipment",
                column: "EquipmentsId",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_AspNetUsers_UserId",
                table: "Equipments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterEquipment_Characters_CharactersId",
                table: "CharacterEquipment");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterEquipment_Equipments_EquipmentsId",
                table: "CharacterEquipment");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_AspNetUsers_UserId",
                table: "Equipments");

            migrationBuilder.DropTable(
                name: "InitativeEquipmentQuantity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Equipments",
                table: "Equipments");

            migrationBuilder.RenameTable(
                name: "Equipments",
                newName: "Equipment");

            migrationBuilder.RenameColumn(
                name: "EquipmentsId",
                table: "CharacterEquipment",
                newName: "EquipmentId");

            migrationBuilder.RenameColumn(
                name: "CharactersId",
                table: "CharacterEquipment",
                newName: "CharacterId");

            migrationBuilder.RenameIndex(
                name: "IX_CharacterEquipment_EquipmentsId",
                table: "CharacterEquipment",
                newName: "IX_CharacterEquipment_EquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Equipments_UserId",
                table: "Equipment",
                newName: "IX_Equipment_UserId");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "CharacterEquipment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Equipment",
                table: "Equipment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterEquipment_Characters_CharacterId",
                table: "CharacterEquipment",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterEquipment_Equipment_EquipmentId",
                table: "CharacterEquipment",
                column: "EquipmentId",
                principalTable: "Equipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_AspNetUsers_UserId",
                table: "Equipment",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
