using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class RequiredAttributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TechPowerIds",
                table: "Creatures");

            migrationBuilder.DropColumn(
                name: "TechPowerIds",
                table: "Characters");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TechPowers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CharacterId",
                table: "TechPowers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatureId",
                table: "TechPowers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ForcePowers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Creatures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Abilities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TechPowers_CharacterId",
                table: "TechPowers",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_TechPowers_CreatureId",
                table: "TechPowers",
                column: "CreatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_TechPowers_Characters_CharacterId",
                table: "TechPowers",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TechPowers_Creatures_CreatureId",
                table: "TechPowers",
                column: "CreatureId",
                principalTable: "Creatures",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TechPowers_Characters_CharacterId",
                table: "TechPowers");

            migrationBuilder.DropForeignKey(
                name: "FK_TechPowers_Creatures_CreatureId",
                table: "TechPowers");

            migrationBuilder.DropIndex(
                name: "IX_TechPowers_CharacterId",
                table: "TechPowers");

            migrationBuilder.DropIndex(
                name: "IX_TechPowers_CreatureId",
                table: "TechPowers");

            migrationBuilder.DropColumn(
                name: "CharacterId",
                table: "TechPowers");

            migrationBuilder.DropColumn(
                name: "CreatureId",
                table: "TechPowers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TechPowers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ForcePowers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Creatures",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "TechPowerIds",
                table: "Creatures",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "TechPowerIds",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Abilities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
