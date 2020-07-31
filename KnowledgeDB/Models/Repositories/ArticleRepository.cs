using KnowledgeDB.Models.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Models.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private EFContext context;
        private readonly ILogger _logger;

        public ArticleRepository(EFContext context, ILogger<ArticleRepository> logger)
        {
            this.context = context;
            _logger = logger;
        }

        public IQueryable<Article> Articles => context.Articles.Include(a => a.RefToTags).ThenInclude(rtt => rtt.ArticelTag);

        public IQueryable<ArticleTag> ArticleTags => context.ArticleTags;

        public async Task<bool> DeleteArticleAsync(Article article)
        {
            if(article != null)
            {
                try
                {
                    context.Remove(article);
                    await context.SaveChangesAsync();
                    return true;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Could'nt delete Article");
                }
            }
            return false;
        }

        public async Task<bool> SaveArticleAsync(Article article)
        {
            Article dbArticle = context.Articles.Include(a => a.RefToTags).ThenInclude(rtt => rtt.ArticelTag).FirstOrDefault(a => a.ArticleId == article.ArticleId);
            if (dbArticle == null)
            {
                //Set Create and Modify Date
                article.CreatedAt = DateTime.Now;
                article.ModifiedAt = DateTime.Now;
                //Just save if Article is new // if Article is new every RefArticleArticleTag must be new too
                context.Add(article);
            }
            else
            {
                article.ModifiedAt = DateTime.Now;
                //Set "new" Values for the existing Article
                context.Entry(dbArticle).CurrentValues.SetValues(article);

                foreach (var rtt in article.RefToTags)
                {
                    //if not existing add new RefArticleArticleTag otherwise nothing has to change
                    RefArticleArticleTag dbArticleTag = dbArticle.RefToTags.FirstOrDefault(dbRtt => $"{dbRtt.ArticelTag.ArticleTagId}{dbRtt.Article.ArticleId}" == $"{rtt.ArticelTag.ArticleTagId}{rtt.Article.ArticleId}");

                    if (dbArticleTag == null)
                    {
                        dbArticle.RefToTags.Add(rtt);
                    }
                }
                //Remove RefToTags from the Existing Article if not listet in the "new" Article
                foreach (var rtt in dbArticle.RefToTags)
                {
                    if (!article.RefToTags.Any(x => $"{x.ArticelTag.ArticleTagId}{x.Article.ArticleId}" == $"{rtt.ArticelTag.ArticleTagId}{rtt.Article.ArticleId}"))
                    {
                        context.Remove(rtt);
                    }
                }
            }

            try
            {
                int result = await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could'nt save Article");
                return false;
            }
        }

        public IEnumerable<Article> SearchArticles(string search)
        {
            var articleIDList = context.Articles.FromSqlInterpolated($"SELECT * FROM search_article_content({search})").Select(a => a.ArticleId).ToList();
            var result = this.Articles.Where(a => articleIDList.Contains(a.ArticleId));

            return result;
        }
    }
}
