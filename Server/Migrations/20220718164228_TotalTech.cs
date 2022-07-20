using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class TotalTech : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxTechPowerLevel",
                table: "Creatures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalTechPowers",
                table: "Creatures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxTechPowerLevel",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalTechPowers",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxTechPowerLevel",
                table: "Creatures");

            migrationBuilder.DropColumn(
                name: "TotalTechPowers",
                table: "Creatures");

            migrationBuilder.DropColumn(
                name: "MaxTechPowerLevel",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalTechPowers",
                table: "Characters");
        }
    }
}
