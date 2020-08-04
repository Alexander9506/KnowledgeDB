using KnowledgeDB.Development;
using KnowledgeDB.Models;
using KnowledgeDB.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Controllers
{
    [Authorize]
    public class ArticleEditController : Controller
    {
        private IArticleRepository articleRepository;

        public ArticleEditController(IArticleRepository articleRepository)
        {
            this.articleRepository = articleRepository;
        }

        public IActionResult EditArticle(int articleId = 0)
        {
            Article article = null;
            if(articleId > 0)
            {
                article = articleRepository.Articles.FirstOrDefault(a => a.ArticleId == articleId);
            }

            //Load empty Article if no IDs found
            article = article ?? new Article();

            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteArticle(int articleId)
        {
            Article toDeleteArticle = articleRepository.Articles.FirstOrDefault(a => a.ArticleId == articleId);

            if (await articleRepository.DeleteArticleAsync(toDeleteArticle))
            {
                TempData["message"] = $"{toDeleteArticle.Title} has been deleted";
                TempData["messageClass"] = "alert-success";
                
            }
            else
            {
                TempData["message"] = $"{toDeleteArticle.Title} could not be deleted";
                return View(nameof(EditArticle), toDeleteArticle);
            }
            return RedirectToAction(nameof(ArticleController.List), "Article");
        }

        [HttpPost]
        public async Task<IActionResult> SaveArticle(Article article, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                article.RefToTags = createArticleTagReferences(article);

                if(await articleRepository.SaveArticleAsync(article))
                {
                    TempData["message"] = $"{article.Title} has been saved";
                    TempData["messageClass"] = "alert-success";
                }
                else
                {
                    TempData["message"] = $"{article.Title} could not be saved";
                    return View(nameof(EditArticle), article);
                }
                if (String.IsNullOrWhiteSpace(returnUrl))
                {
                    returnUrl = Url.Action("ShowArticle", "Article", article);
                }

                return Redirect(returnUrl);
            }

            return View(nameof(EditArticle),article);
        }

        private List<RefArticleArticleTag> createArticleTagReferences(Article article)
        {
            List<RefArticleArticleTag> finalTagList = null;

            if(article.RefToTags != null && article.RefToTags.Count > 0)
            {
                //Get list of Articles
                IEnumerable<ArticleTag> rawTagList = article.RefToTags.Select(rtt => rtt.ArticelTag);
                //Get list of TagNames
                IEnumerable<String> tagListNames = rawTagList.Select(t => t.Name);

                //load ArticleTag Objects with the tagName from database 
                List<ArticleTag> dbTagList = articleRepository.ArticleTags.Where(at => tagListNames.Contains(at.Name)).ToList();
                //create the new References to the Article and the Tag
                finalTagList = rawTagList.Select(tag => new RefArticleArticleTag { ArticelTag = dbTagList.FirstOrDefault(dbTag => dbTag.Name == tag.Name) ?? tag, Article = article }).ToList();
            }

            return finalTagList;
        }
    }
}
