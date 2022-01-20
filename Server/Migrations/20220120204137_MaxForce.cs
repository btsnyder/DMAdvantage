using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class MaxForce : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxForcePowerLevel",
                table: "Creatures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalForcePowers",
                table: "Creatures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxForcePowerLevel",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalForcePowers",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxForcePowerLevel",
                table: "Creatures");

            migrationBuilder.DropColumn(
                name: "TotalForcePowers",
                table: "Creatures");

            migrationBuilder.DropColumn(
                name: "MaxForcePowerLevel",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TotalForcePowers",
                table: "Characters");
        }
    }
}
