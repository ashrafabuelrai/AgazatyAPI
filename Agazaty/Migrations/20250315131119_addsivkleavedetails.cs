using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agazaty.Migrations
{
    public partial class addsivkleavedetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Days",
                table: "SickLeaves",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "SickLeaves",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "SickLeaves",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "governorate",
                table: "SickLeaves",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Days",
                table: "SickLeaves");

            migrationBuilder.DropColumn(
                name: "State",
                table: "SickLeaves");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "SickLeaves");

            migrationBuilder.DropColumn(
                name: "governorate",
                table: "SickLeaves");
        }
    }
}
