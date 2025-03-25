using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agazaty.Migrations
{
    public partial class adddisability : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SickLeaves_UserID",
                table: "SickLeaves");

            migrationBuilder.DropIndex(
                name: "IX_NormalLeaves_UserID",
                table: "NormalLeaves");

            migrationBuilder.DropIndex(
                name: "IX_CasualLeaves_UserId",
                table: "CasualLeaves");

            migrationBuilder.AddColumn<bool>(
                name: "Disability",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_SickLeaves_UserID_StartDate_EndDate",
                table: "SickLeaves",
                columns: new[] { "UserID", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_NormalLeaves_UserID_StartDate_EndDate",
                table: "NormalLeaves",
                columns: new[] { "UserID", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CasualLeaves_UserId_StartDate_EndDate",
                table: "CasualLeaves",
                columns: new[] { "UserId", "StartDate", "EndDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SickLeaves_UserID_StartDate_EndDate",
                table: "SickLeaves");

            migrationBuilder.DropIndex(
                name: "IX_NormalLeaves_UserID_StartDate_EndDate",
                table: "NormalLeaves");

            migrationBuilder.DropIndex(
                name: "IX_CasualLeaves_UserId_StartDate_EndDate",
                table: "CasualLeaves");

            migrationBuilder.DropColumn(
                name: "Disability",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_SickLeaves_UserID",
                table: "SickLeaves",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_NormalLeaves_UserID",
                table: "NormalLeaves",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CasualLeaves_UserId",
                table: "CasualLeaves",
                column: "UserId");
        }
    }
}
