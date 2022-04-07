using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class addcredits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PropertyDescription",
                table: "Weapons");

            migrationBuilder.AddColumn<int>(
                name: "Credits",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Credits",
                table: "Characters");

            migrationBuilder.AddColumn<string>(
                name: "PropertyDescription",
                table: "Weapons",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
