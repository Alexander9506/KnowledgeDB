using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Models.ViewModels.Components.Tag
{
    public class TagMenuViewModel
    {
        public IEnumerable<ArticleTag> Tags { get; set; }
        public String CurrentTag { get; set; }
    }
}
