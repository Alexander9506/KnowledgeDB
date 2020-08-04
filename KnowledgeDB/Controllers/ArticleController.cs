using Ganss.XSS;
using KnowledgeDB.Models;
using KnowledgeDB.Models.Repositories;
using KnowledgeDB.Models.ViewModels;
using KnowledgeDB.Models.ViewModels.Article;
using Markdig;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace KnowledgeDB.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleRepository articleRepository;
        private readonly ILogger _logger;
        public int EntriesPerPage = 10;

        public ArticleController(IArticleRepository repository, ILogger<ArticleController> logger)
        {
            this.articleRepository = repository;
            this._logger = logger;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(List));
        }

        public IActionResult ShowArticle(int articleId)
        {
            Article article = articleRepository.Articles.FirstOrDefault(a => a.ArticleId == articleId);

            if(article == null)
            {
                TempData["message"] = "Article could not be opened";
                return RedirectToAction(nameof(List));
            }
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
            ViewBag.Title = "Article Overview";

            IEnumerable<Article> articles;
            ArticleTag filterTag = null;


            if(articleTagId < 1)
            {
                articles = articleRepository.Articles;
            }
            else
            {
                filterTag = articleRepository.ArticleTags.FirstOrDefault(a => a.ArticleTagId == articleTagId);
                articles = articleRepository.Articles.Where(a => a.RefToTags != null && a.RefToTags.Select(t => t.ArticelTag.ArticleTagId).Contains(articleTagId));
            }

            string pageName = filterTag != null ? $"Articles with Tag: {filterTag.Name}" : "Last Modified Articles";
            ArticleListViewModel listModel = CreateListViewModel(page, articles, pageName, articleTagId);

            return View(listModel);
        }

        public IActionResult Search(String search)
        {
            int page = 1;
            var articles = articleRepository.SearchArticles(search).ToList();

            ArticleListViewModel listViewModel = CreateListViewModel(page, articles, $"You searched for: {search}");
            return View("List", listViewModel);
        }

        private IEnumerable<Article> getArticlesForPage(IEnumerable<Article> articles, int page) => 
            articles.OrderByDescending(a => a.ModifiedAt).Skip((page - 1) * EntriesPerPage).Take(EntriesPerPage);
        
        private ArticleListViewModel CreateListViewModel(int page, IEnumerable<Article> articles, string pageName, int articleTagId = 0)
        {
            ArticleListViewModel listModel = new ArticleListViewModel
            {
                ListViewModel = new ListViewModel
                {
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = page,
                        EntriesPerPage = EntriesPerPage,
                        TotalEntries = articles.Count(),
                    },
                    PartialViewName = "ArticleCard",
                    Entries = getArticlesForPage(articles, page)
                },
                ArticleTagId = articleTagId,
                PageName = pageName
            };
            return listModel;
        }

    }
}
