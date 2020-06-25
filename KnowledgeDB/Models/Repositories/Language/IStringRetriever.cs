using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Models.Repositories.Language
{
    public interface IStringRetriever
    {
        IQueryable<StringEntry> StringEntries { get; }
        IQueryable<StringEntryGroup> StringEntryGroups { get; }
        IQueryable<StringKey> StringKeys { get; }
        IQueryable<LanguagePack> LanguagePacks { get; }
    }
}
