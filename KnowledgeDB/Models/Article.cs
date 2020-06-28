using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Models
{
    public class Article : IChangeInformationBase
    {
        public int ArticleId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Content { get; set; }
        public string Summary { get; set; }
        public string TeaserImagePath { get; set; }
        public string ArticleImagePath { get; set; }
        public List<RefArticleArticleTag> RefToTags { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
