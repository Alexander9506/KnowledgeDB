using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Models.Repositories
{
    public interface IArticleRepository
    {
        IQueryable<Article> Articles { get; }
        IQueryable<ArticleTag> ArticleTags { get; }

        Task<bool> SaveArticleAsync(Article article);
        Task<bool> DeleteArticleAsync(Article article);

        IEnumerable<Article> SearchArticles(string searchString);
    }
}
