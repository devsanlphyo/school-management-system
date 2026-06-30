using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainSchoolsManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class RenameTeacherIdToUserId_Attendance_LeaveRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_AspNetUsers_TeacherId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_TeacherId",
                table: "LeaveRequests");

            migrationBuilder.RenameColumn(
                name: "TeacherId",
                table: "LeaveRequests",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveRequests_TeacherId",
                table: "LeaveRequests",
                newName: "IX_LeaveRequests_UserId");

            migrationBuilder.RenameColumn(
                name: "TeacherId",
                table: "Attendances",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_TeacherId",
                table: "Attendances",
                newName: "IX_Attendances_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_AspNetUsers_UserId",
                table: "Attendances",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_UserId",
                table: "LeaveRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_AspNetUsers_UserId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_UserId",
                table: "LeaveRequests");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "LeaveRequests",
                newName: "TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveRequests_UserId",
                table: "LeaveRequests",
                newName: "IX_LeaveRequests_TeacherId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Attendances",
                newName: "TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_UserId",
                table: "Attendances",
                newName: "IX_Attendances_TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_AspNetUsers_TeacherId",
                table: "Attendances",
                column: "TeacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_TeacherId",
                table: "LeaveRequests",
                column: "TeacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
