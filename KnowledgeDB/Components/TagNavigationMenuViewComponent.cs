using KnowledgeDB.Models;
using KnowledgeDB.Models.Repositories;
using KnowledgeDB.Models.ViewModels.Components.Tag;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Components
{
    /// <summary>
    /// Lists the top most used Tags
    /// </summary>
    public class TagNavigationMenuViewComponent : ViewComponent
    {
        private readonly IArticleRepository _articleRepository;

        public TagNavigationMenuViewComponent(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(int showEntries = 4)
        {
            IEnumerable<ArticleTag> mostUsedTags = await _articleRepository.ArticleTags
                .OrderByDescending (at => at.RefToArticles == null ? 0 : at.RefToArticles.Count)
                .Take(showEntries)
                .ToListAsync();
            
            string currentTagName = "";
            if (int.TryParse(Request.Query["articleTagId"].ToString(), out int currentTagId))
            {
                currentTagName = mostUsedTags.FirstOrDefault(mut => mut.ArticleTagId == currentTagId)?.Name ?? "";
            }

            TagMenuViewModel viewModel = new TagMenuViewModel
            {
                Tags = mostUsedTags,
                CurrentTag = currentTagName
            };

            return View(viewModel);
        }
    }
    
}
