using KnowledgeDB.Controllers;
using KnowledgeDB.Models;
using KnowledgeDB.Models.Repositories;
using KnowledgeDB.Models.ViewModels.Article;
using KnowledgeDB.Tests.Helper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace KnowledgeDB.Tests.Controllers
{
    public class ArticleControllerTests
    {
        [Fact]
        public void CanShowArticleOfPage()
        {
            Mock<IArticleRepository> repo = new Mock<IArticleRepository>();
            repo.Setup(r => r.Articles).Returns(new Article[]
            {
                new Article{ ArticleId = 1, Title = "Article1", ModifiedAt = DateTime.Now},
                new Article{ ArticleId = 2, Title = "Article2", ModifiedAt = DateTime.Now.AddMinutes(-20)},
                new Article{ ArticleId = 3, Title = "Article3", ModifiedAt = DateTime.Now.AddMinutes(-40)},
            }.AsQueryable<Article>());

            //Two Entries per page
            ArticleController target = new ArticleController(repo.Object, null);
            target.EntriesPerPage = 2;

            //the second page should only have the "Article3" because the Entries are ordered by modifiedAt Date
            ArticleListViewModel viewModel = target.List(page: 2).GetViewModel<ArticleListViewModel>();
            object[] result = viewModel.ListViewModel.Entries.ToArray();

            Assert.Single(result);
            Article a = Assert.IsType<Article>(result[0]);
            if(a != null)
            {
                Assert.Equal("Article3", a.Title);
            }
        }

        [Fact]
        public void CanShowArticlesOfTag()
        {
            Mock<IArticleRepository> repo = new Mock<IArticleRepository>();

            ArticleTag[] articleTags = new ArticleTag[]
            {
                new ArticleTag{ArticleTagId = 1, Name = "TestTag"},
                new ArticleTag{ArticleTagId = 2, Name = "TestTag2"},
            };

            repo.Setup(r => r.ArticleTags).Returns(articleTags.AsQueryable<ArticleTag>());


            repo.Setup(r => r.Articles).Returns(new Article[]
            {
                new Article{ ArticleId = 1, Title = "Article1", ModifiedAt = DateTime.Now, RefToTags = new List<RefArticleArticleTag>
                {
                    new RefArticleArticleTag{ArticelTag = articleTags[0]}}
                },
                new Article{ ArticleId = 2, Title = "Article2", ModifiedAt = DateTime.Now.AddMinutes(-20), RefToTags = new List<RefArticleArticleTag>
                {
                    new RefArticleArticleTag{ArticelTag = articleTags[1]}}
                },
                new Article{ ArticleId = 3, Title = "Article3", ModifiedAt = DateTime.Now.AddMinutes(-40), RefToTags = new List<RefArticleArticleTag>
                {
                    new RefArticleArticleTag{ArticelTag = articleTags[1]}}
                },
                new Article{ ArticleId = 4, Title = "Article4", ModifiedAt = DateTime.Now.AddMinutes(-42)},
            }.AsQueryable<Article>());


            //Two Entries per page
            ArticleController target = new ArticleController(repo.Object, null);
            target.EntriesPerPage = 2;
            
            ArticleListViewModel viewModel = target.List(articleTagId: 2).GetViewModel<ArticleListViewModel>();
            object[] result = viewModel.ListViewModel.Entries.ToArray();

            Assert.Equal(2, result.Length);
            Article a = Assert.IsType<Article>(result[0]);
            if (a != null)
            {
                Assert.Equal("Article2", a.Title);
                Assert.Equal("Article3", ((Article)result[1]).Title);
            }
            
        }

        [Fact]
        public void CanSearchForArticleContent()
        {
            Mock<IArticleRepository> repo = new Mock<IArticleRepository>();
            repo.Setup(r => r.SearchArticles("Test")).Returns(new Article[]
            {
                new Article{ ArticleId = 1, Title = "Article1", ModifiedAt = DateTime.Now},
                new Article{ ArticleId = 2, Title = "Article2", ModifiedAt = DateTime.Now.AddMinutes(-20)},
            }.AsQueryable<Article>());

            //Two Entries per page
            ArticleController target = new ArticleController(repo.Object, null);
            target.EntriesPerPage = 2;

            //the second page should only have the "Article3" because the Entries are ordered by modifiedAt Date
            ArticleListViewModel viewModel =target.Search("Test").GetViewModel<ArticleListViewModel>();
            object[] result = viewModel.ListViewModel.Entries.ToArray();

            Assert.Equal(2, result.Length);
            Assert.Equal("Article1", ((Article)result[0]).Title);
            Assert.Equal("Article2", ((Article)result[1]).Title);
        }

        [Fact]
        public void CanSearchForArticleTag()
        {
            //TODO: Function in ArticleController not yet implemented
            Assert.True(false);
        }

    }
}
