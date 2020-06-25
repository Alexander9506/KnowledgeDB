using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Models.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private EFContext context;

        public ArticleRepository(EFContext context)
        {
            this.context = context;
        }

        public IQueryable<Article> Articles => context.Articles.Include(a => a.RefToTags).ThenInclude(rtt => rtt.ArticelTag);

        public IQueryable<ArticleTag> ArticleTags => context.ArticleTags;

        public async Task<bool> SaveArticleAsync(Article article)
        {
            //if (article.Tags.Count > 0)
            //{
            //    AddArticleTagsToContext(article.Tags);
            //}
            context.Update(article);
            
            try
            {
                int result = await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                //TODO: Logging
                return false;
            }
        }

        //private void AddArticleTagsToContext(List<RefArticleArticleTag> tags)
        //{
        //    foreach (RefArticleArticleTag refTag in tags)
        //    {
        //        addArticleRefToContext(refTag);
        //    }
        //}

        //private void addArticleRefToContext(RefArticleArticleTag refTag)
        //{
        //    if (refTag.ArticelTag?.ArticleTagId > 0 && refTag.Article?.ArticleId > 0)
        //    {
        //        context.Update(refTag);
        //    }
        //    else
        //    {
        //        context.Add(refTag);
        //    }
        //    addArticleTagToContext(refTag.ArticelTag);
        //}
        //private void addArticleTagToContext(ArticleTag tag)
        //{
        //    if(tag.ArticleTagId > 0)
        //    {
        //        context.Update(tag);
        //    }
        //    else
        //    {
        //        context.Add(tag);
        //    }
        //}
    }
}
