using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Models.ViewModels.Article
{
    public class ArticleListViewModel
    {
        public ListViewModel ListViewModel { get; set; }
        public int ArticleTagId { get; set; }
        public string PageName { get; set; }
    }
}
