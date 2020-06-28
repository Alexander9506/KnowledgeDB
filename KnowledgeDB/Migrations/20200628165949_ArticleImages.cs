using Microsoft.EntityFrameworkCore.Migrations;

namespace KnowledgeDB.Migrations
{
    public partial class ArticleImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArticleImagePath",
                table: "Articles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeaserImagePath",
                table: "Articles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArticleImagePath",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "TeaserImagePath",
                table: "Articles");
        }
    }
}
