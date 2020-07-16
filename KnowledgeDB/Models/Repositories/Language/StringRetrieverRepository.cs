using KnowledgeDB.Models.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace KnowledgeDB.Models.Repositories.Language
{
    public class StringRetrieverRepository : IStringRetriever
    {
        private EFLanguageContext context;

        public StringRetrieverRepository(EFLanguageContext context)
        {
            this.context = context;
        }

        public IQueryable<StringEntry> StringEntries => context.StringEntries.Include(se => se.LanguagePack).Include(se => se.Key);
        public IQueryable<StringEntryGroup> StringEntryGroups => context.StringEntryGroups.Include(se => se.LanguagePack).Include(se => se.Key).Include(se => se.Entries);
        public IQueryable<StringKey> StringKeys => context.StringKeys.Include(sk => sk.Entries);
        public IQueryable<LanguagePack> LanguagePacks => context.LanguagePacks;
    }
}
