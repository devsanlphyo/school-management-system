using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainSchoolsManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeedPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedPosts_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedPostComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeedPostId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedPostComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedPostComments_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeedPostComments_FeedPosts_FeedPostId",
                        column: x => x.FeedPostId,
                        principalTable: "FeedPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedPostMedias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeedPostId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedPostMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedPostMedias_FeedPosts_FeedPostId",
                        column: x => x.FeedPostId,
                        principalTable: "FeedPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedPostReactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeedPostId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedPostReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedPostReactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeedPostReactions_FeedPosts_FeedPostId",
                        column: x => x.FeedPostId,
                        principalTable: "FeedPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeedPostComments_AuthorId",
                table: "FeedPostComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedPostComments_FeedPostId",
                table: "FeedPostComments",
                column: "FeedPostId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedPostMedias_FeedPostId",
                table: "FeedPostMedias",
                column: "FeedPostId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedPostReactions_FeedPostId_UserId",
                table: "FeedPostReactions",
                columns: new[] { "FeedPostId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeedPostReactions_UserId",
                table: "FeedPostReactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedPosts_AuthorId",
                table: "FeedPosts",
                column: "AuthorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedPostComments");

            migrationBuilder.DropTable(
                name: "FeedPostMedias");

            migrationBuilder.DropTable(
                name: "FeedPostReactions");

            migrationBuilder.DropTable(
                name: "FeedPosts");
        }
    }
}
