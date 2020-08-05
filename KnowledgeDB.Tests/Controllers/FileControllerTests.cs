
using KnowledgeDB.Controllers;
using KnowledgeDB.Infrastructure;

using KnowledgeDB.Models;
using KnowledgeDB.Models.Repositories;
using KnowledgeDB.Models.ViewModels.File;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KnowledgeDB.Tests.Controllers
{
    public class FileControllerTests
    {
        [Fact]
        public void CanGetFilesByFilter()
        {
            Mock<IFileRepository> fileRepo = new Mock<IFileRepository>();
            Mock<IWebHostEnvironment> env = new Mock<IWebHostEnvironment>();
            Mock<IFileHelper> fileHelper = new Mock<IFileHelper>();

            fileRepo.Setup(fr => fr.FileContainers).Returns(new FileContainer[]
            {
                new FileContainer{FileContainerId = 1, FileDisplayName ="File1.png", FileType = "image/jpg", FilePathFull="\\KnowledgeDB\\KnowledgeDB\\wwwroot\\images\\hjkhjkl.png"},
                new FileContainer { FileContainerId = 2, FileDisplayName = "File2.png", FileType = "text/txt", FilePathFull="\\KnowledgeDB\\KnowledgeDB\\wwwroot\\images\\hjkd.png" },
                new FileContainer{FileContainerId = 3, FileDisplayName ="File3.png", FileType = "image/png", FilePathFull="\\KnowledgeDB\\KnowledgeDB\\wwwroot\\images\\asdf.png"},
            }.AsQueryable<FileContainer>());

            env.Setup(e => e.WebRootPath).Returns("\\KnowledgeDB\\KnowledgeDB\\wwwroot");

            FileController target = new FileController(fileRepo.Object, env.Object, fileHelper.Object);
            FileFilter filter = new FileFilter() { FileType = "image" };
            IActionResult result = target.GetFiles(filter);

            JsonResult jsonResult = Assert.IsType<JsonResult>(result);
            string expectedJsonContent = "{\"ContentType\":null,\"SerializerSettings\":null,\"StatusCode\":null,\"Value\":[{\"Id\":1,\"DisplayName\":\"File1.png\",\"FileUrl\":\"/images/hjkhjkl.png\",\"FileType\":\"image/jpg\",\"FileDescription\":null},{\"Id\":3,\"DisplayName\":\"File3.png\",\"FileUrl\":\"/images/asdf.png\",\"FileType\":\"image/png\",\"FileDescription\":null}]}";
            string jsonContentString = JsonConvert.SerializeObject(jsonResult);

            Assert.Equal(expectedJsonContent, jsonContentString);
        }

        [Fact]
        public async void CanDeleteFile()
        {
            Mock<IFileRepository> fileRepo = new Mock<IFileRepository>();
            Mock<IWebHostEnvironment> env = new Mock<IWebHostEnvironment>();
            Mock<IFileHelper> fileHelper = new Mock<IFileHelper>();

            FileContainer fileContainerToDelete = new FileContainer { FileContainerId = 2, FileDisplayName = "File2.png" };
            fileRepo.Setup(fr => fr.FileContainers).Returns(new FileContainer[]
            {
                new FileContainer{FileContainerId = 1, FileDisplayName ="File1.png"},
                fileContainerToDelete,
                new FileContainer{FileContainerId = 3, FileDisplayName ="File3.png"},
            }.AsQueryable<FileContainer>());

            fileHelper.Setup(fh => fh.DeleteFileAsync(fileContainerToDelete)).Returns(Task.FromResult(true));

            FileController target = new FileController(fileRepo.Object, env.Object, fileHelper.Object);
            IActionResult result = await target.DeleteFile(2);

            fileHelper.Verify(fh => fh.DeleteFileAsync(fileContainerToDelete));
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void CannotDeleteNonExistingFile()
        {
            Mock<IFileRepository> fileRepo = new Mock<IFileRepository>();
            Mock<IWebHostEnvironment> env = new Mock<IWebHostEnvironment>();
            Mock<IFileHelper> fileHelper = new Mock<IFileHelper>();

            
            fileRepo.Setup(fr => fr.FileContainers).Returns(new FileContainer[]
            {
                new FileContainer{FileContainerId = 1, FileDisplayName ="File1.png"},
                new FileContainer { FileContainerId = 2, FileDisplayName = "File2.png" },
                new FileContainer{FileContainerId = 3, FileDisplayName ="File3.png"},
            }.AsQueryable<FileContainer>());

            FileController target = new FileController(fileRepo.Object, env.Object, fileHelper.Object);
            IActionResult result = await target.DeleteFile(4);

            fileHelper.Verify(fh => fh.DeleteFileAsync(It.IsAny<FileContainer>()), Times.Never);
            Assert.IsNotType<OkResult>(result);
        }
    }
}
