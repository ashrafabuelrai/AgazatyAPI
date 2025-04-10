using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agazaty.Migrations
{
    public partial class addimage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PermitLeaveImages_LeaveId",
                table: "PermitLeaveImages");

            migrationBuilder.CreateIndex(
                name: "IX_PermitLeaveImages_LeaveId",
                table: "PermitLeaveImages",
                column: "LeaveId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PermitLeaveImages_LeaveId",
                table: "PermitLeaveImages");

            migrationBuilder.CreateIndex(
                name: "IX_PermitLeaveImages_LeaveId",
                table: "PermitLeaveImages",
                column: "LeaveId");
        }
    }
}
