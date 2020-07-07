using Microsoft.EntityFrameworkCore.Migrations;

namespace KnowledgeDB.Migrations
{
    public partial class FileContainerGuiId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GuiId",
                table: "FileContainers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuiId",
                table: "FileContainers");
        }
    }
}
