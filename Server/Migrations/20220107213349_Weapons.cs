using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class Weapons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WeaponsCache",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeaponsCache",
                table: "Characters");
        }
    }
}
