using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fir.App.Migrations
{
    public partial class updateOrderOrderItemTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isAccepted",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isCompleted",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isAccepted",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "isCompleted",
                table: "Orders");
        }
    }
}
