using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class ClassAndFTPoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ForcePoints",
                table: "Creatures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TechPoints",
                table: "Creatures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Class",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ForcePoints",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TechPoints",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForcePoints",
                table: "Creatures");

            migrationBuilder.DropColumn(
                name: "TechPoints",
                table: "Creatures");

            migrationBuilder.DropColumn(
                name: "Class",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "ForcePoints",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "TechPoints",
                table: "Characters");
        }
    }
}
