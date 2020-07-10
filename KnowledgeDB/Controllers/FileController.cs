using KnowledgeDB.Models;
using KnowledgeDB.Models.Repositories;
using KnowledgeDB.Models.ViewModels.File;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                string localFilePath = container.FilePathFull;
                if(await fileRepository.DeleteFileContainer(container))
                {
                    try
                    {
                        System.IO.File.Delete(localFilePath);
                        return Ok();
                    }
                    catch(Exception e)
                    {
                        //TODO: Log
                    }
                }
            }
            return BadRequest();
        }

        [HttpPost]
        public JsonResult GetFiles([FromBody]FileFilter filter)
        {
            IEnumerable<FileContainer> rawFileContainer = fileRepository.FileContainers;
            if(filter?.FileType != null)
            {
                string fileType = filter.FileType.ToLower();
                rawFileContainer = rawFileContainer.Where(fc => fc.FileType.Substring(0, Math.Min(fc.FileType.Length, fileType.Length)).ToLower() == fileType);
            }

            IEnumerable<FilePreviewViewModel> files = rawFileContainer.ToList().Select(f => new FilePreviewViewModel
            {
                Id = f.FileContainerId,
                FileUrl = "/" + Path.GetRelativePath(environment.WebRootPath, f.FilePathFull).Replace("\\","/"),
                DisplayName = f.FileDisplayName,
                FileDescription = f.FileDescription,
            });
            
            return new JsonResult(files);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<FileViewModel> files)
        {
            if (files.Any())
            {
                long size = files.Sum(f => f.FormFile.Length);
                String basePath = Path.Combine(environment.WebRootPath, configuration.GetValue<String>("ImagePath"));
                String[] allowedFileExtensions = configuration.GetSection("AllowedFileExtensions").Get<String[]>();

                foreach (var fileViewModel in files)
                {
                    IFormFile formFile = fileViewModel.FormFile;

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
                    container.GuiId = fileViewModel.GUIID;

                    if (container != null)
                    {
                        try {
                            using (var stream = System.IO.File.Create(container.FilePathFull))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                            await fileRepository.SaveFileContainer(container);
                            
                            return Ok(new { count = files.Count, size });
                        } catch (Exception e) {
                            //TODO: Logging
                        }
                    }
                }
            }
            return BadRequest();
        }

        
    }
}
