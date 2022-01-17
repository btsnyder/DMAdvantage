using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class Concentration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConcentrationCache",
                table: "Encounters",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConcentrationCache",
                table: "Encounters");
        }
    }
}
