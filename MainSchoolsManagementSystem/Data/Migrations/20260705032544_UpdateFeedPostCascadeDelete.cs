using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainSchoolsManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFeedPostCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedPostComments_AspNetUsers_AuthorId",
                table: "FeedPostComments");

            migrationBuilder.DropForeignKey(
                name: "FK_FeedPostReactions_AspNetUsers_UserId",
                table: "FeedPostReactions");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedPostComments_AspNetUsers_AuthorId",
                table: "FeedPostComments",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FeedPostReactions_AspNetUsers_UserId",
                table: "FeedPostReactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedPostComments_AspNetUsers_AuthorId",
                table: "FeedPostComments");

            migrationBuilder.DropForeignKey(
                name: "FK_FeedPostReactions_AspNetUsers_UserId",
                table: "FeedPostReactions");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedPostComments_AspNetUsers_AuthorId",
                table: "FeedPostComments",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FeedPostReactions_AspNetUsers_UserId",
                table: "FeedPostReactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
