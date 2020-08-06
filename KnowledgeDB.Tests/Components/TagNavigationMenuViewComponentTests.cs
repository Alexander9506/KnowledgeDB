using AngleSharp.Io;
using KnowledgeDB.Components;
using KnowledgeDB.Models;
using KnowledgeDB.Models.Repositories;
using KnowledgeDB.Models.ViewModels.Components.Tag;
using KnowledgeDB.Tests.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace KnowledgeDB.Tests.Components
{
    public class TagNavigationMenuViewComponentTests
    {
        [Fact]
        public async void CanShowMostUsedTags()
        {
            Mock<IArticleRepository> repoMock = new Mock<IArticleRepository>();

            ArticleTag[] articleTags = new ArticleTag[]
            {
                new ArticleTag{ArticleTagId = 1, Name = "TestTag",
                    RefToArticles = new List<RefArticleArticleTag>(){
                        new RefArticleArticleTag(),
                    }
                },
                new ArticleTag{ArticleTagId = 2, Name = "TestTag2",
                    RefToArticles = new List<RefArticleArticleTag>(){
                        new RefArticleArticleTag(),
                        new RefArticleArticleTag(),
                        new RefArticleArticleTag(),
                    }
                },
                new ArticleTag{ArticleTagId = 3, Name = "TestTag3",
                    RefToArticles = new List<RefArticleArticleTag>(){
                        new RefArticleArticleTag(),
                        new RefArticleArticleTag(),
                    }
                },
            };

            repoMock.Setup(r => r.ArticleTags).Returns(articleTags.AsQueryable().GetDbSet<ArticleTag>().Object);

            Mock<HttpContext> contextMock = new Mock<HttpContext>();
            Mock<HttpRequest> requestMock = new Mock<HttpRequest>();
            contextMock.Setup(c => c.Request).Returns(requestMock.Object);
            requestMock.Setup(r => r.Query["articleTagId"]).Returns("2");

            TagNavigationMenuViewComponent target = new TagNavigationMenuViewComponent(repoMock.Object);
            target.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext
                {
                    HttpContext = contextMock.Object
                }
            };
            var invokeResult = await target.InvokeAsync(2);
            TagMenuViewModel model = (TagMenuViewModel)(invokeResult as ViewViewComponentResult).ViewData.Model;

            Assert.Equal(2, model.Tags.Count());
            Assert.Equal("TestTag2", model.Tags.First().Name);
        }

        [Fact]
        public async void CanShowMostUsedTagsWithLessThenRequestedAvailable()
        {
            Mock<IArticleRepository> repoMock = new Mock<IArticleRepository>();

            ArticleTag[] articleTags = new ArticleTag[]
            {
                new ArticleTag{ArticleTagId = 1, Name = "TestTag",
                    RefToArticles = new List<RefArticleArticleTag>(){
                        new RefArticleArticleTag(),
                    }
                },
                new ArticleTag{ArticleTagId = 2, Name = "TestTag2",
                    RefToArticles = new List<RefArticleArticleTag>(){
                        new RefArticleArticleTag(),
                        new RefArticleArticleTag(),
                        new RefArticleArticleTag(),
                    }
                },
                new ArticleTag{ArticleTagId = 3, Name = "TestTag3",
                    RefToArticles = new List<RefArticleArticleTag>(){
                        new RefArticleArticleTag(),
                        new RefArticleArticleTag(),
                    }
                },
            };

            repoMock.Setup(r => r.ArticleTags).Returns(articleTags.AsQueryable().GetDbSet<ArticleTag>().Object);

            Mock<HttpContext> contextMock = new Mock<HttpContext>();
            Mock<HttpRequest> requestMock = new Mock<HttpRequest>();
            contextMock.Setup(c => c.Request).Returns(requestMock.Object);
            requestMock.Setup(r => r.Query["articleTagId"]).Returns("2");

            TagNavigationMenuViewComponent target = new TagNavigationMenuViewComponent(repoMock.Object);
            target.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext
                {
                    HttpContext = contextMock.Object
                }
            };
            var invokeResult = await target.InvokeAsync(5);
            TagMenuViewModel model = (TagMenuViewModel)(invokeResult as ViewViewComponentResult).ViewData.Model;

            Assert.Equal(3, model.Tags.Count());
            Assert.Equal("TestTag2", model.Tags.First().Name);
        }

        [Fact]
        public async void CanHighlightActiveTag() {
            Mock<IArticleRepository> repoMock = new Mock<IArticleRepository>();

            ArticleTag[] articleTags = new ArticleTag[]
            {
                new ArticleTag{ArticleTagId = 1, Name = "TestTag",
                    RefToArticles = new List<RefArticleArticleTag>(){
                        new RefArticleArticleTag(),
                    }
                },
                new ArticleTag{ArticleTagId = 2, Name = "TestTag2",
                    RefToArticles = new List<RefArticleArticleTag>(){
                        new RefArticleArticleTag(),
                        new RefArticleArticleTag(),
                        new RefArticleArticleTag(),
                    }
                },
                new ArticleTag{ArticleTagId = 3, Name = "TestTag3",
                    RefToArticles = new List<RefArticleArticleTag>(){
                        new RefArticleArticleTag(),
                        new RefArticleArticleTag(),
                    }
                },
            };

            repoMock.Setup(r => r.ArticleTags).Returns(articleTags.AsQueryable().GetDbSet<ArticleTag>().Object);

            Mock<HttpContext> contextMock = new Mock<HttpContext>();
            Mock<HttpRequest> requestMock = new Mock<HttpRequest>();
            contextMock.Setup(c => c.Request).Returns(requestMock.Object);
            requestMock.Setup(r => r.Query["articleTagId"]).Returns("2");

            TagNavigationMenuViewComponent target = new TagNavigationMenuViewComponent(repoMock.Object);
            target.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext
                {
                    HttpContext = contextMock.Object
                }
            };
            var invokeResult = await target.InvokeAsync(2);
            TagMenuViewModel model = (TagMenuViewModel)(invokeResult as ViewViewComponentResult).ViewData.Model;
            
            Assert.Equal("TestTag2", model.CurrentTag);
        }

        
    }

}
