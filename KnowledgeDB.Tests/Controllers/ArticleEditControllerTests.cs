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
using System.Text;
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
            HttpContext context = new DefaultHttpContext();
            var tempData = new TempDataDictionary(context, Mock.Of<ITempDataProvider>());

            Mock<IArticleRepository> repoMock = new Mock<IArticleRepository>();
            repoMock.Setup(r => r.Articles).Returns(new Article[] {
                new Article{ ArticleId = 1, Title = "Article1", ModifiedAt = DateTime.Now},
                new Article{ ArticleId = 2, Title = "Article2", ModifiedAt = DateTime.Now.AddMinutes(-20)},
                new Article{ ArticleId = 3, Title = "Article3", ModifiedAt = DateTime.Now.AddMinutes(-40)},
            }.AsQueryable<Article>());

            ArticleEditController target = new ArticleEditController(repoMock.Object);
            target.TempData = tempData;


            ViewResult viewResult = target.EditArticle(4) as ViewResult;
            Article result = target.EditArticle(4).GetViewModel<Article>();
            
            String message = viewResult.TempData["message"].ToString();

            Assert.NotNull(result);
            Assert.NotEqual(4, result.ArticleId);
            Assert.Contains("could'nt be found/opened", message);
        }

        [Fact]
        public void CanSaveValidChanges()
        {

        }

        [Fact]
        public void CannotSaveInvalidChanges()
        {

        }

        [Fact]
        public void CanDeleteArticle()
        {

        }
        [Fact]
        public void CannotDeleteNonExistentArticle()
        {

        }
    }
}
