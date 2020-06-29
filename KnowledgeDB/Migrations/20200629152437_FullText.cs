using Microsoft.EntityFrameworkCore.Migrations;

namespace KnowledgeDB.Migrations
{
    public partial class FullText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Create Fulltext Table
            migrationBuilder.Sql(@"
                CREATE TABLE public.""FulltextArticles""
                    (
                        ""FulltextArticleId"" SERIAL PRIMARY KEY,
                        ""ArticleId"" integer NOT NULL,
                        ""Tokens"" tsvector NOT NULL,
                        CONSTRAINT ""ArticleId"" FOREIGN KEY (""ArticleId"")
                            REFERENCES public.""Articles"" (""ArticleId"") MATCH SIMPLE
                            ON UPDATE CASCADE
                            ON DELETE CASCADE
                    )
                    WITH (
                        OIDS = FALSE
                    )
                    TABLESPACE pg_default;
                    ALTER TABLE public.""FulltextArticles""
                        OWNER to postgres;
                    COMMENT ON TABLE public.""FulltextArticles""
                        IS 'Used to hold Article Fulltext';
                    CREATE INDEX article_fulltext_index ON ""FulltextArticles""
                    USING GIN
                    (
                        ""Tokens"" 
                    );
            ");

            //Create the Fulltext Trigger Functions
            migrationBuilder.Sql(
                @"
                    CREATE OR REPLACE FUNCTION create_articles_fulltext() RETURNS trigger AS $trigger_bound$
	                    BEGIN
	                    INSERT INTO ""FulltextArticles"" (""ArticleId"", ""Tokens"")
		                    (SELECT ""ArticleId"", to_tsvector(""Content"") FROM ""Articles"" WHERE ""ArticleId"" = NEW.""ArticleId"");
	                    RETURN NULL;
	                    END;
                    $trigger_bound$
                    LANGUAGE plpgsql;
                    CREATE OR REPLACE FUNCTION update_articles_fulltext() RETURNS trigger AS $trigger_bound$
	                    BEGIN
	                    UPDATE ""FulltextArticles"" 
			                    SET ""Tokens"" = (SELECT to_tsvector(""Content"") FROM ""Articles""
			                    WHERE ""ArticleId"" = NEW.""ArticleId"");
	                    RETURN NULL;
	                    END;
                    $trigger_bound$
                    LANGUAGE plpgsql;
                ");

            migrationBuilder.Sql(@"
                    CREATE TRIGGER InsertArticle AFTER INSERT
	                    ON ""Articles""
	                    FOR EACH ROW
	                    EXECUTE PROCEDURE create_articles_fulltext();
	
                    CREATE TRIGGER UpdateArticle AFTER UPDATE OF ""Content""
	                    ON ""Articles""
	                    FOR EACH ROW
	                    EXECUTE PROCEDURE update_articles_fulltext();
            ");


            //Create the Fulltext Searchfunction

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION search_article_content(searchString text) RETURNS TABLE(""ArticleId"" integer) AS $$
	                BEGIN
		                RETURN QUERY
			                SELECT ""Articles"".""ArticleId""  FROM ""Articles"" WHERE ""Articles"".""Content"" @@ to_tsquery(searchString);
	                END;
                $$
                LANGUAGE plpgsql;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Fulltext Table
            migrationBuilder.Sql(@"
                DROP INDEX article_fulltext_index;
                DROP TABLE public.""FulltextArticles"";
            ");

            //Trigger
            migrationBuilder.Sql(@"
                DROP TRIGGER InsertArticle;
                DROP TRIGGER UpdateArticle;
                DROP FUNCTION update_articles_fulltext();
                DROP FUNCTION create_articles_fulltext();
            ");
        }
    }
}
