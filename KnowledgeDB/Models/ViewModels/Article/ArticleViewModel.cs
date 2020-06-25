using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KnowledgeDB.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace KnowledgeDB.Models.ViewModels.Article
{
    public class ArticleViewModel
    {
        public Models.Article Article { get; set; }
        public String HtmlContent { get; set; }
    }
}
