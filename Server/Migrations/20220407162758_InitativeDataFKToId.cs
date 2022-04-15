using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class InitativeDataFKToId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InitativeData_Characters_CharacterId",
                table: "InitativeData");

            migrationBuilder.DropForeignKey(
                name: "FK_InitativeData_Creatures_CreatureId",
                table: "InitativeData");

            migrationBuilder.DropForeignKey(
                name: "FK_InitativeData_Encounters_EncounterId",
                table: "InitativeData");

            migrationBuilder.AlterColumn<Guid>(
                name: "EncounterId",
                table: "InitativeData",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatureId",
                table: "InitativeData",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CharacterId",
                table: "InitativeData",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InitativeData_Characters_CharacterId",
                table: "InitativeData",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InitativeData_Creatures_CreatureId",
                table: "InitativeData",
                column: "CreatureId",
                principalTable: "Creatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InitativeData_Encounters_EncounterId",
                table: "InitativeData",
                column: "EncounterId",
                principalTable: "Encounters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InitativeData_Characters_CharacterId",
                table: "InitativeData");

            migrationBuilder.DropForeignKey(
                name: "FK_InitativeData_Creatures_CreatureId",
                table: "InitativeData");

            migrationBuilder.DropForeignKey(
                name: "FK_InitativeData_Encounters_EncounterId",
                table: "InitativeData");

            migrationBuilder.AlterColumn<Guid>(
                name: "EncounterId",
                table: "InitativeData",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatureId",
                table: "InitativeData",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CharacterId",
                table: "InitativeData",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_InitativeData_Characters_CharacterId",
                table: "InitativeData",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InitativeData_Creatures_CreatureId",
                table: "InitativeData",
                column: "CreatureId",
                principalTable: "Creatures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InitativeData_Encounters_EncounterId",
                table: "InitativeData",
                column: "EncounterId",
                principalTable: "Encounters",
                principalColumn: "Id");
        }
    }
}
