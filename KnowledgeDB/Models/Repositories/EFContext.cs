using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Models.Repositories
{
    public class EFContext : DbContext
    {
        private IConfiguration configuration;
        
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleTag> ArticleTags { get; set; }

        public EFContext(DbContextOptions<EFContext> options, IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql(configuration.GetConnectionString("KnowledgeDB"));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Many to many relation between Article and ArticleTag
            modelBuilder.Entity<RefArticleArticleTag>()
                .HasKey(raat => new { raat.ArticleTagId, raat.ArticleId });

            modelBuilder.Entity<RefArticleArticleTag>()
                .HasOne(raat => raat.Article)
                .WithMany(a => a.RefToTags)
                .HasForeignKey(raat => raat.ArticleId);

            modelBuilder.Entity<RefArticleArticleTag>()
                .HasOne(raat => raat.ArticelTag)
                .WithMany(at => at.RefToArticles)
                .HasForeignKey(raat => raat.ArticleTagId);
        }
    }
}
