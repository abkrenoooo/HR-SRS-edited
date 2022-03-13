using Microsoft.EntityFrameworkCore.Migrations;

namespace HR_High.Data.Migrations
{
    public partial class changeEmpType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "Salarry",
                table: "Employees",
                newName: "Salary");

            migrationBuilder.AddColumn<string>(
                name: "TypeState",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypeState",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "Salary",
                table: "Employees",
                newName: "Salarry");

            migrationBuilder.AddColumn<bool>(
                name: "Type",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
