using Microsoft.EntityFrameworkCore.Migrations;

namespace HR_High.Data.Migrations
{
    public partial class @as : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateOfContract",
                table: "Attendances",
                newName: "Date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Attendances",
                newName: "DateOfContract");
        }
    }
}
