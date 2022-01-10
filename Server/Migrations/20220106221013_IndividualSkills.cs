using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class IndividualSkills : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpertSkills",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "ProficientSkills",
                table: "Characters");

            migrationBuilder.AddColumn<bool>(
                name: "Acrobatics",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AnimalHandling",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Athletics",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CharismaSave",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ConstitutionSave",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deception",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DexteritySave",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Insight",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IntelligenceSave",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Intimidation",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Investigation",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Lore",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Medicine",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Nature",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Perception",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Performance",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Persuasion",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Piloting",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SleightOfHand",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Stealth",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "StrengthSave",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Survival",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Technology",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WisdomSave",
                table: "Characters",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Acrobatics",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "AnimalHandling",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Athletics",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CharismaSave",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "ConstitutionSave",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Deception",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "DexteritySave",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Insight",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "IntelligenceSave",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Intimidation",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Investigation",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Lore",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Medicine",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Nature",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Perception",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Performance",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Persuasion",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Piloting",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "SleightOfHand",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Stealth",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "StrengthSave",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Survival",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Technology",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "WisdomSave",
                table: "Characters");

            migrationBuilder.AddColumn<string>(
                name: "ExpertSkills",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProficientSkills",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
