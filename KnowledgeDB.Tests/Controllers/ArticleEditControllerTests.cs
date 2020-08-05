using AngleSharp;
using KnowledgeDB.Controllers;
using KnowledgeDB.Models;
using KnowledgeDB.Models.Repositories;
using KnowledgeDB.Tests.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KnowledgeDB.Tests.Controllers
{
    public class ArticleEditControllerTests
    {
        [Fact]
        public void CanEditArticle()
        {
            Mock<IArticleRepository> repo = new Mock<IArticleRepository>();
            repo.Setup(r => r.Articles).Returns(new Article[] {
                new Article{ ArticleId = 1, Title = "Article1", ModifiedAt = DateTime.Now},
                new Article{ ArticleId = 2, Title = "Article2", ModifiedAt = DateTime.Now.AddMinutes(-20)},
                new Article{ ArticleId = 3, Title = "Article3", ModifiedAt = DateTime.Now.AddMinutes(-40)},
            }.AsQueryable<Article>());

            ArticleEditController target = new ArticleEditController(repo.Object);
            Article result = target.EditArticle(2).GetViewModel<Article>();

            Assert.NotNull(result);
            Assert.Equal("Article2", result.Title);
        }

        [Fact]
        public void CannotEditNonExistentArticle()
        {
            Mock<IArticleRepository> repoMock = new Mock<IArticleRepository>();
            repoMock.Setup(r => r.Articles).Returns(new Article[] {
                new Article{ ArticleId = 1, Title = "Article1", ModifiedAt = DateTime.Now},
                new Article{ ArticleId = 2, Title = "Article2", ModifiedAt = DateTime.Now.AddMinutes(-20)},
                new Article{ ArticleId = 3, Title = "Article3", ModifiedAt = DateTime.Now.AddMinutes(-40)},
            }.AsQueryable<Article>());

            TempDataDictionary tdd = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            ArticleEditController target = new ArticleEditController(repoMock.Object);
            target.TempData = tdd;

            ViewResult viewResult = target.EditArticle(4) as ViewResult;
            Article result = target.EditArticle(4).GetViewModel<Article>();
            
            String message = viewResult.TempData["message"].ToString();

            Assert.NotNull(result);
            Assert.NotEqual(4, result.ArticleId);
            Assert.Contains("could'nt be found/opened", message);
        }

        [Fact]
        public async void CanSaveValidChanges()
        {
            Mock<IArticleRepository> repoMock = new Mock<IArticleRepository>();
            repoMock.Setup(r => r.Articles).Returns(new Article[] {
                new Article{ ArticleId = 1, Title = "Article1", ModifiedAt = DateTime.Now},
                new Article{ ArticleId = 2, Title = "Article2", ModifiedAt = DateTime.Now.AddMinutes(-20)},
                new Article{ ArticleId = 3, Title = "Article3", ModifiedAt = DateTime.Now.AddMinutes(-40)},
            }.AsQueryable<Article>());
            
            Article a = new Article { ArticleId = 4, Title = "Article4" };

            repoMock.Setup(r => r.SaveArticleAsync(a)).Returns(Task.FromResult(true));

            ArticleEditController target = new ArticleEditController(repoMock.Object);
            target.TempData = Mock.Of<ITempDataDictionary>();

            IActionResult result = await target.SaveArticle(a, "/test/url");

            repoMock.Verify(r => r.SaveArticleAsync(a));

            Assert.IsType<RedirectResult>(result);
            Assert.Equal("/test/url", (result as RedirectResult).Url);
        }

        [Fact]
        public async void CannotSaveInvalidChanges()
        {
            Mock<IArticleRepository> repoMock = new Mock<IArticleRepository>();
            repoMock.Setup(r => r.Articles).Returns(new Article[] {
                new Article{ ArticleId = 1, Title = "Article1", ModifiedAt = DateTime.Now},
                new Article{ ArticleId = 2, Title = "Article2", ModifiedAt = DateTime.Now.AddMinutes(-20)},
                new Article{ ArticleId = 3, Title = "Article3", ModifiedAt = DateTime.Now.AddMinutes(-40)},
            }.AsQueryable<Article>());

            repoMock.Setup(r => r.SaveArticleAsync(It.IsAny<Article>())).Returns(Task.FromResult(true));

            ArticleEditController target = new ArticleEditController(repoMock.Object);
            target.TempData = Mock.Of<ITempDataDictionary>();
            target.ModelState.AddModelError("error", "error");

            Article a = new Article { ArticleId = 4, Title = "Article4" };
            IActionResult result = await target.SaveArticle(a, "/test/url");

            repoMock.Verify(r => r.SaveArticleAsync(It.IsAny<Article>()), Times.Never());

            Assert.IsType<ViewResult>(result);
            Assert.Equal(nameof(ArticleEditController.EditArticle), (result as ViewResult).ViewName) ;
        }

        [Fact]
        public async void CanDeleteArticle()
        {
            Mock<IArticleRepository> repoMock = new Mock<IArticleRepository>();

            Article articleToDelete = new Article { ArticleId = 2, Title = "Article2", ModifiedAt = DateTime.Now.AddMinutes(-20) };
            repoMock.Setup(r => r.Articles).Returns(new Article[]
            {
                new Article{ ArticleId = 1, Title = "Article1", ModifiedAt = DateTime.Now},
                articleToDelete,
                new Article{ ArticleId = 3, Title = "Article3", ModifiedAt = DateTime.Now.AddMinutes(-40)},
            }.AsQueryable());

            repoMock.Setup(r => r.DeleteArticleAsync(articleToDelete)).Returns(Task.FromResult(true));

            ArticleEditController target = new ArticleEditController(repoMock.Object);
            target.TempData = Mock.Of<ITempDataDictionary>();
            IActionResult result = await target.DeleteArticle(2);

            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(ArticleController.List), redirectResult?.ActionName ?? "");
        }
        [Fact]
        public async void CannotDeleteNonExistentArticle()
        {
            Mock<IArticleRepository> repoMock = new Mock<IArticleRepository>();

            repoMock.Setup(r => r.Articles).Returns(new Article[]
            {
                new Article{ ArticleId = 1, Title = "Article1", ModifiedAt = DateTime.Now},
                 new Article { ArticleId = 2, Title = "Article2", ModifiedAt = DateTime.Now.AddMinutes(-20) },
                new Article{ ArticleId = 3, Title = "Article3", ModifiedAt = DateTime.Now.AddMinutes(-40)},
            }.AsQueryable());
            
            repoMock.Setup(r => r.DeleteArticleAsync(It.IsAny<Article>())).Returns(Task.FromResult(false));

            ArticleEditController target = new ArticleEditController(repoMock.Object);
            target.TempData = Mock.Of<ITempDataDictionary>();
            IActionResult result = await target.DeleteArticle(4);

            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(nameof(ArticleEditController.EditArticle), viewResult?.ViewName);
        }
    }
}
