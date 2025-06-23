using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Posts.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UOW : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostWithTextCategories_PostTextCategory_PostTextCategoryId",
                table: "PostWithTextCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_PostWithTextCategories_Posts_PostId",
                table: "PostWithTextCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostWithTextCategories",
                table: "PostWithTextCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostTextCategory",
                table: "PostTextCategory");

            migrationBuilder.RenameTable(
                name: "PostWithTextCategories",
                newName: "PostsWithTextCategories");

            migrationBuilder.RenameTable(
                name: "PostTextCategory",
                newName: "PostTextCategories");

            migrationBuilder.RenameIndex(
                name: "IX_PostWithTextCategories_PostTextCategoryId",
                table: "PostsWithTextCategories",
                newName: "IX_PostsWithTextCategories_PostTextCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostsWithTextCategories",
                table: "PostsWithTextCategories",
                columns: new[] { "PostId", "PostTextCategoryId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostTextCategories",
                table: "PostTextCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostsWithTextCategories_PostTextCategories_PostTextCategoryId",
                table: "PostsWithTextCategories",
                column: "PostTextCategoryId",
                principalTable: "PostTextCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostsWithTextCategories_Posts_PostId",
                table: "PostsWithTextCategories",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostsWithTextCategories_PostTextCategories_PostTextCategoryId",
                table: "PostsWithTextCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_PostsWithTextCategories_Posts_PostId",
                table: "PostsWithTextCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostTextCategories",
                table: "PostTextCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostsWithTextCategories",
                table: "PostsWithTextCategories");

            migrationBuilder.RenameTable(
                name: "PostTextCategories",
                newName: "PostTextCategory");

            migrationBuilder.RenameTable(
                name: "PostsWithTextCategories",
                newName: "PostWithTextCategories");

            migrationBuilder.RenameIndex(
                name: "IX_PostsWithTextCategories_PostTextCategoryId",
                table: "PostWithTextCategories",
                newName: "IX_PostWithTextCategories_PostTextCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostTextCategory",
                table: "PostTextCategory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostWithTextCategories",
                table: "PostWithTextCategories",
                columns: new[] { "PostId", "PostTextCategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PostWithTextCategories_PostTextCategory_PostTextCategoryId",
                table: "PostWithTextCategories",
                column: "PostTextCategoryId",
                principalTable: "PostTextCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostWithTextCategories_Posts_PostId",
                table: "PostWithTextCategories",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
