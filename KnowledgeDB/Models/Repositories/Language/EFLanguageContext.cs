using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;

namespace KnowledgeDB.Models.Repositories.Language
{
    public class EFLanguageContext : DbContext
    {
        public DbSet<StringEntry> StringEntries { get; set; }
        public DbSet<StringEntryGroup> StringEntryGroups { get; set; }
        public DbSet<StringKey> StringKeys { get; set; }
        public DbSet<LanguagePack> LanguagePacks { get; set; }

        public EFLanguageContext(DbContextOptions<EFLanguageContext> options) : base(options)
        {
        }
    }
}
