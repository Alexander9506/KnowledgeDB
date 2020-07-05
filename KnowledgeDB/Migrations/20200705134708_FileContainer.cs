using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KnowledgeDB.Migrations
{
    public partial class FileContainer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileContainers",
                columns: table => new
                {
                    FileContainerId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileDisplayName = table.Column<string>(nullable: true),
                    FileType = table.Column<string>(nullable: true),
                    FilePathFull = table.Column<string>(nullable: false),
                    FileDescription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileContainers", x => x.FileContainerId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileContainers");
        }
    }
}
