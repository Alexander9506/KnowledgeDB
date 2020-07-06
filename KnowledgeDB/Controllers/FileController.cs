using KnowledgeDB.Models;
using KnowledgeDB.Models.Repositories;
using KnowledgeDB.Models.ViewModels.File;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Controllers
{
    public class FileController : Controller
    {
        private IFileRepository fileRepository;
        private IWebHostEnvironment environment;
        private IConfiguration configuration;

        public FileController(IFileRepository fileRepository, IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.fileRepository = fileRepository;
            this.environment = environment;
            this.configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFile(int id)
        {
            FileContainer container = fileRepository.FileContainers.FirstOrDefault(f => f.FileContainerId == id);

            if(container != null)
            {
                if(await fileRepository.DeleteFileContainer(container))
                {
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpGet]
        public JsonResult GetImages()
        {
            var rawFileContainer = fileRepository.FileContainers.ToList();
            IEnumerable<FilePreviewViewModel> files = fileRepository.FileContainers.ToList().Select(f => new FilePreviewViewModel
            {
                Id = f.FileContainerId,
                FileUrl = "/" + Path.GetRelativePath(environment.WebRootPath, f.FilePathFull).Replace("\\","/"),
                DisplayName = f.FileDisplayName,
                FileDescription = f.FileDescription,
            });
            
            return new JsonResult(files);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            if (files.Any())
            {
                long size = files.Sum(f => f.Length);
                String basePath = Path.Combine(environment.WebRootPath, configuration.GetValue<String>("ImagePath"));
                String[] allowedFileExtensions = configuration.GetSection("AllowedFileExtensions").Get<String[]>();

                foreach (var formFile in files)
                {
                    //New random Filename
                    string originalFileExtension = Path.GetExtension(formFile.FileName).Replace(".", string.Empty);
                    string newFileName = Path.GetRandomFileName();
                    if (allowedFileExtensions.Contains(originalFileExtension))
                    {
                        if (newFileName.Contains("."))
                        {
                            newFileName = newFileName.Substring(0, newFileName.IndexOf(".") - 1);
                            newFileName += "." + originalFileExtension;
                        }
                    }

                    //Create and Save FileContainer
                    FileContainer container = FileContainerFactory.CreateFileContainer(formFile, basePath, newFileName);
                    await fileRepository.SaveFileContainer(container);

                    if (container != null)
                    {
                        using (var stream = System.IO.File.Create(container.FilePathFull))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }
                return Ok(new { count = files.Count, size });
            }
            return BadRequest();
        }
    }
}
