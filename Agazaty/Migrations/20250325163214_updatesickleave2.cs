using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agazaty.Migrations
{
    public partial class updatesickleave2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RespononseDone",
                table: "SickLeaves",
                newName: "ResponseDoneFinal");

            migrationBuilder.RenameColumn(
                name: "SickLeavesCount",
                table: "AspNetUsers",
                newName: "NonChronicSickLeavesCount");

            migrationBuilder.AddColumn<bool>(
                name: "Certified",
                table: "SickLeaves",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Chronic",
                table: "SickLeaves",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RespononseDoneForMedicalCommitte",
                table: "SickLeaves",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Certified",
                table: "SickLeaves");

            migrationBuilder.DropColumn(
                name: "Chronic",
                table: "SickLeaves");

            migrationBuilder.DropColumn(
                name: "RespononseDoneForMedicalCommitte",
                table: "SickLeaves");

            migrationBuilder.RenameColumn(
                name: "ResponseDoneFinal",
                table: "SickLeaves",
                newName: "RespononseDone");

            migrationBuilder.RenameColumn(
                name: "NonChronicSickLeavesCount",
                table: "AspNetUsers",
                newName: "SickLeavesCount");
        }
    }
}
