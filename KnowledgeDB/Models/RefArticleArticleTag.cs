using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Models
{
    public class RefArticleArticleTag
    {
        public int ArticleId { get; set; }
        public Article Article { get; set; }
        public int ArticleTagId { get; set; }
        public ArticleTag ArticelTag { get; set; }
    }
}
