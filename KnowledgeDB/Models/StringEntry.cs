using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Models
{
    public class StringEntry
    {
        public int ID { get; set; }
        public StringKey Key { get; set; }
        public String Text { get; set; }
        public LanguagePack LanguagePack { get; set; }
    }

    public class StringEntryGroup : StringEntry
    {
        public IEnumerable<StringEntry> Entries { get; set; }
    }

    public class StringKey
    {
        public int ID { get; set; }
        public String KeyName { get; set; }
        public IEnumerable<StringEntry> Entries { get; set; }
    }

    public class LanguagePack
    {
        public int ID { get; set; }
        public string NativeLanguageName { get; set; }
        public string EnglishLanguageName { get; set; }
        public IEnumerable<StringEntry> StringEntries { get; set; }
    }
}
