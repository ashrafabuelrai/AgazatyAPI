using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agazaty.Migrations
{
    public partial class addsikeleavedetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeAddress",
                table: "SickLeaves");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployeeAddress",
                table: "SickLeaves",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
