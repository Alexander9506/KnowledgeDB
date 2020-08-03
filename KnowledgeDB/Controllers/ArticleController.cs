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
        private readonly IFileRepository fileRepository;
        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration configuration;
        private readonly ILogger _logger;
        private int entriesPerPage = 10;

        public ArticleController(IArticleRepository repository, 
            IWebHostEnvironment environment,
            IConfiguration configuration, 
            IFileRepository fileRepository,
            ILogger<ArticleController> logger)
        {
            this.articleRepository = repository;
            this.fileRepository = fileRepository;
            this.environment = environment;
            this.configuration = configuration;
            this._logger = logger;
        }

        public IActionResult Index()
        {
            throw new Exception("Test Error");
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
                articles = articleRepository.Articles.Where(a => a.RefToTags.Select(t => t.ArticelTag.ArticleTagId).Contains(articleTagId));
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
                    Entries = articles.OrderByDescending(a => a.ModifiedAt).Skip((page - 1) * entriesPerPage).Take(entriesPerPage)
                },
                ArticleTagId = articleTagId,
                PageName = filterTag != null ? $"Articles with Tag: {filterTag.Name}" : "Last Modified Articles"
            };
            return View(listModel);
        }

        public IActionResult Search(String search)
        {
            int page = 1;
            var articles = articleRepository.SearchArticles(search).ToList();

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
                ArticleTagId = 0,
                PageName = $"You searched for: {search}"
            };
            return View("List", listModel);
        }

    }
}
