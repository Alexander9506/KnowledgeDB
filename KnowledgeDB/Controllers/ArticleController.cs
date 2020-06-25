using Ganss.XSS;
using KnowledgeDB.Models;
using KnowledgeDB.Models.Repositories;
using KnowledgeDB.Models.ViewModels;
using KnowledgeDB.Models.ViewModels.Article;
using Markdig;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Controllers
{
    public class ArticleController : Controller
    {
        private IArticleRepository repository;
        private int entriesPerPage = 10;

        public ArticleController(IArticleRepository repository)
        {
            this.repository = repository;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(List));
        }

        public IActionResult ShowArticle(int ArticleId)
        {
            Article article = repository.Articles.FirstOrDefault(a => a.ArticleId == ArticleId);

            //Setup Markdown Pipline
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            //convert Markdown to HTML
            string htmlContent = Markdown.ToHtml(article.Content, pipeline);
            
            //Sanitize HTML
            var sanitizer = new HtmlSanitizer();
            htmlContent = sanitizer.Sanitize(htmlContent);

            ArticleViewModel model = new ArticleViewModel
            {
                Article = article,
                HtmlContent = htmlContent
            };

            return View(model);
        }

        public IActionResult List(int page = 1, int articleTagId = 0)
        {
            //TODO: set DefaultValue and improve
            ViewBag.Title = "Article Overview";

            IEnumerable<Article> articles;

            if(articleTagId < 1)
            {
                articles = repository.Articles;
            }
            else
            {
                articles = repository.Articles.Where(a => a.RefToTags.Select(t => t.ArticelTag.ArticleTagId).Contains(articleTagId));
            }

            ArticleListViewModel listModel = new ArticleListViewModel
            {
                ListViewModel = new ListViewModel
                {
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = page,
                        EntriesPerPage = entriesPerPage,
                        TotalEntries = articles.Count(),
                    },
                    PartialViewName = "ArticleCard",
                    Entries = articles.OrderBy(a => a.ModifiedAt).Skip((page - 1) * entriesPerPage).Take(entriesPerPage)
                },
                ArticleTagId = articleTagId
            };
            return View(listModel);
        }

        public IActionResult Search(String search)
        {
            throw new NotImplementedException();
        }
    }
}
