using System.Collections.Generic;

namespace KnowledgeDB.Models
{
    public class ArticleTag
    {
        public int ArticleTagId{ get; set; }
        public string Name { get; set; }
        public List<RefArticleArticleTag> RefToArticles { get; set; }
    }
}
