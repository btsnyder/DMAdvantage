using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class AddingPowers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ForcePowerIds",
                table: "Creatures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TechPowerIds",
                table: "Creatures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ForcePowerIds",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TechPowerIds",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForcePowerIds",
                table: "Creatures");

            migrationBuilder.DropColumn(
                name: "TechPowerIds",
                table: "Creatures");

            migrationBuilder.DropColumn(
                name: "ForcePowerIds",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TechPowerIds",
                table: "Characters");
        }
    }
}
