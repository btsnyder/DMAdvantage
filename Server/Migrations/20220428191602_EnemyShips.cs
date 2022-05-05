using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMAdvantage.Server.Migrations
{
    public partial class EnemyShips : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnemyShips_AspNetUsers_UserId",
                table: "EnemyShips");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipInitativeData_EnemyShips_EnemyShipId",
                table: "ShipInitativeData");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipInitativeData_EnemyShips_PlayerShipId",
                table: "ShipInitativeData");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipShipWeapon_EnemyShips_ShipsId",
                table: "ShipShipWeapon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EnemyShips",
                table: "EnemyShips");

            migrationBuilder.RenameTable(
                name: "EnemyShips",
                newName: "Ship");

            migrationBuilder.RenameIndex(
                name: "IX_EnemyShips_UserId",
                table: "Ship",
                newName: "IX_Ship_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ship",
                table: "Ship",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ship_AspNetUsers_UserId",
                table: "Ship",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipInitativeData_Ship_EnemyShipId",
                table: "ShipInitativeData",
                column: "EnemyShipId",
                principalTable: "Ship",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipInitativeData_Ship_PlayerShipId",
                table: "ShipInitativeData",
                column: "PlayerShipId",
                principalTable: "Ship",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipShipWeapon_Ship_ShipsId",
                table: "ShipShipWeapon",
                column: "ShipsId",
                principalTable: "Ship",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ship_AspNetUsers_UserId",
                table: "Ship");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipInitativeData_Ship_EnemyShipId",
                table: "ShipInitativeData");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipInitativeData_Ship_PlayerShipId",
                table: "ShipInitativeData");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipShipWeapon_Ship_ShipsId",
                table: "ShipShipWeapon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ship",
                table: "Ship");

            migrationBuilder.RenameTable(
                name: "Ship",
                newName: "EnemyShips");

            migrationBuilder.RenameIndex(
                name: "IX_Ship_UserId",
                table: "EnemyShips",
                newName: "IX_EnemyShips_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EnemyShips",
                table: "EnemyShips",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EnemyShips_AspNetUsers_UserId",
                table: "EnemyShips",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipInitativeData_EnemyShips_EnemyShipId",
                table: "ShipInitativeData",
                column: "EnemyShipId",
                principalTable: "EnemyShips",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipInitativeData_EnemyShips_PlayerShipId",
                table: "ShipInitativeData",
                column: "PlayerShipId",
                principalTable: "EnemyShips",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipShipWeapon_EnemyShips_ShipsId",
                table: "ShipShipWeapon",
                column: "ShipsId",
                principalTable: "EnemyShips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
