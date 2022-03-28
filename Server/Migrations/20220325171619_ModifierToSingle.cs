using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class ModifierToSingle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Modifiers",
                table: "WeaponProperties",
                newName: "Modifier");

            migrationBuilder.AddColumn<string>(
                name: "PropertyDescription",
                table: "Weapons",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PropertyDescription",
                table: "Weapons");

            migrationBuilder.RenameColumn(
                name: "Modifier",
                table: "WeaponProperties",
                newName: "Modifiers");
        }
    }
}
