﻿using Ganss.XSS;
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
using System.IO;

namespace KnowledgeDB.Controllers
{
    public class ArticleController : Controller
    {
        private IArticleRepository repository;
        private IWebHostEnvironment environment;
        private IConfiguration configuration;
        private int entriesPerPage = 10;

        public ArticleController(IArticleRepository repository, IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.repository = repository;
            this.environment = environment;
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(List));
        }

        public IActionResult ShowArticle(int articleId)
        {
            Article article = repository.Articles.FirstOrDefault(a => a.ArticleId == articleId);

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
                articles = repository.Articles;
            }
            else
            {
                filterTag = repository.ArticleTags.FirstOrDefault(a => a.ArticleTagId == articleTagId);
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
            var articles = repository.SearchArticles(search).ToList();

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
        public IActionResult Test()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            String basePath = Path.Combine(environment.WebRootPath, configuration.GetValue<string>("ImagePath"));
            foreach (var formFile in files)
            {
                string filePath = Path.Combine(basePath, Path.GetRandomFileName());
                using (var stream = System.IO.File.Create(filePath))
                {
                    await formFile.CopyToAsync(stream);
                }
            }
            return Ok(new { count = files.Count, size });
        }
    }
}
