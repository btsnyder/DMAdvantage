using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class EncounterToInitativeData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CharacterIds",
                table: "Encounters");

            migrationBuilder.DropColumn(
                name: "CreatureIds",
                table: "Encounters");

            migrationBuilder.AddColumn<string>(
                name: "DataCache",
                table: "Encounters",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataCache",
                table: "Encounters");

            migrationBuilder.AddColumn<string>(
                name: "CharacterIds",
                table: "Encounters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatureIds",
                table: "Encounters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
