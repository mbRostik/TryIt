using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Posts.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UOW2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostWithPhotoCategories_PostPhotoCategory_PostPhotoCategoryId",
                table: "PostWithPhotoCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_PostWithPhotoCategories_Posts_PostId",
                table: "PostWithPhotoCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostWithPhotoCategories",
                table: "PostWithPhotoCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostPhotoCategory",
                table: "PostPhotoCategory");

            migrationBuilder.RenameTable(
                name: "PostWithPhotoCategories",
                newName: "PostsWithPhotoCategories");

            migrationBuilder.RenameTable(
                name: "PostPhotoCategory",
                newName: "PostPhotoCategories");

            migrationBuilder.RenameIndex(
                name: "IX_PostWithPhotoCategories_PostPhotoCategoryId",
                table: "PostsWithPhotoCategories",
                newName: "IX_PostsWithPhotoCategories_PostPhotoCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostsWithPhotoCategories",
                table: "PostsWithPhotoCategories",
                columns: new[] { "PostId", "PostPhotoCategoryId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostPhotoCategories",
                table: "PostPhotoCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostsWithPhotoCategories_PostPhotoCategories_PostPhotoCategoryId",
                table: "PostsWithPhotoCategories",
                column: "PostPhotoCategoryId",
                principalTable: "PostPhotoCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostsWithPhotoCategories_Posts_PostId",
                table: "PostsWithPhotoCategories",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostsWithPhotoCategories_PostPhotoCategories_PostPhotoCategoryId",
                table: "PostsWithPhotoCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_PostsWithPhotoCategories_Posts_PostId",
                table: "PostsWithPhotoCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostsWithPhotoCategories",
                table: "PostsWithPhotoCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostPhotoCategories",
                table: "PostPhotoCategories");

            migrationBuilder.RenameTable(
                name: "PostsWithPhotoCategories",
                newName: "PostWithPhotoCategories");

            migrationBuilder.RenameTable(
                name: "PostPhotoCategories",
                newName: "PostPhotoCategory");

            migrationBuilder.RenameIndex(
                name: "IX_PostsWithPhotoCategories_PostPhotoCategoryId",
                table: "PostWithPhotoCategories",
                newName: "IX_PostWithPhotoCategories_PostPhotoCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostWithPhotoCategories",
                table: "PostWithPhotoCategories",
                columns: new[] { "PostId", "PostPhotoCategoryId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostPhotoCategory",
                table: "PostPhotoCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostWithPhotoCategories_PostPhotoCategory_PostPhotoCategoryId",
                table: "PostWithPhotoCategories",
                column: "PostPhotoCategoryId",
                principalTable: "PostPhotoCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostWithPhotoCategories_Posts_PostId",
                table: "PostWithPhotoCategories",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
