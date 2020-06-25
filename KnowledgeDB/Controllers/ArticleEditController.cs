using KnowledgeDB.Models;
using KnowledgeDB.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Controllers
{
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

        public IActionResult DeleteArticle(int articleId)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> SaveArticle(Article article)
        {
            if (ModelState.IsValid)
            {
                await articleRepository.SaveArticleAsync(article);
                TempData["message"] = $"{article.Title} has been saved";
                TempData["messageClass"] = "alert-success";

                return RedirectToAction(nameof(EditArticle));
            }

            return View(nameof(EditArticle),article);
        }
    }
}
