using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Posts.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class PostCategoriesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostPhotoCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostPhotoCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostTextCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostTextCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostWithPhotoCategories",
                columns: table => new
                {
                    PostId = table.Column<int>(type: "int", nullable: false),
                    PostPhotoCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostWithPhotoCategories", x => new { x.PostId, x.PostPhotoCategoryId });
                    table.ForeignKey(
                        name: "FK_PostWithPhotoCategories_PostPhotoCategory_PostPhotoCategoryId",
                        column: x => x.PostPhotoCategoryId,
                        principalTable: "PostPhotoCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostWithPhotoCategories_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostWithTextCategories",
                columns: table => new
                {
                    PostId = table.Column<int>(type: "int", nullable: false),
                    PostTextCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostWithTextCategories", x => new { x.PostId, x.PostTextCategoryId });
                    table.ForeignKey(
                        name: "FK_PostWithTextCategories_PostTextCategory_PostTextCategoryId",
                        column: x => x.PostTextCategoryId,
                        principalTable: "PostTextCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostWithTextCategories_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostWithPhotoCategories_PostPhotoCategoryId",
                table: "PostWithPhotoCategories",
                column: "PostPhotoCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PostWithTextCategories_PostTextCategoryId",
                table: "PostWithTextCategories",
                column: "PostTextCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostWithPhotoCategories");

            migrationBuilder.DropTable(
                name: "PostWithTextCategories");

            migrationBuilder.DropTable(
                name: "PostPhotoCategory");

            migrationBuilder.DropTable(
                name: "PostTextCategory");
        }
    }
}
