using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KnowledgeDB.Migrations.EFLanguage
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LanguagePacks",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NativeLanguageName = table.Column<string>(nullable: true),
                    EnglishLanguageName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguagePacks", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StringKeys",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KeyName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StringKeys", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StringEntries",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KeyID = table.Column<int>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    LanguagePackID = table.Column<int>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    StringEntryGroupID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StringEntries", x => x.ID);
                    table.ForeignKey(
                        name: "FK_StringEntries_StringKeys_KeyID",
                        column: x => x.KeyID,
                        principalTable: "StringKeys",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StringEntries_LanguagePacks_LanguagePackID",
                        column: x => x.LanguagePackID,
                        principalTable: "LanguagePacks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StringEntries_StringEntries_StringEntryGroupID",
                        column: x => x.StringEntryGroupID,
                        principalTable: "StringEntries",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StringEntries_KeyID",
                table: "StringEntries",
                column: "KeyID");

            migrationBuilder.CreateIndex(
                name: "IX_StringEntries_LanguagePackID",
                table: "StringEntries",
                column: "LanguagePackID");

            migrationBuilder.CreateIndex(
                name: "IX_StringEntries_StringEntryGroupID",
                table: "StringEntries",
                column: "StringEntryGroupID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StringEntries");

            migrationBuilder.DropTable(
                name: "StringKeys");

            migrationBuilder.DropTable(
                name: "LanguagePacks");
        }
    }
}
