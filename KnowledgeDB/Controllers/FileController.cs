using KnowledgeDB.Infrastructure;
using KnowledgeDB.Models;
using KnowledgeDB.Models.Repositories;
using KnowledgeDB.Models.ViewModels.File;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class FileController : Controller
    {
        private readonly IFileRepository fileRepository;
        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration _configuration;
        private readonly IFileHelper _fileHelper;

        public FileController(IFileRepository fileRepository, IWebHostEnvironment environment, IConfiguration configuration, IFileHelper fileHelper)
        {
            this.fileRepository = fileRepository;
            this.environment = environment;
            this._configuration = configuration;
            this._fileHelper = fileHelper;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFile(int id)
        {
            FileContainer container = fileRepository.FileContainers.FirstOrDefault(f => f.FileContainerId == id);

            if(container != null)
            {
                if (await _fileHelper.DeleteFileAsync(container))
                {
                    return Ok();
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
                FileUrl = "/" + Path.GetRelativePath(environment.WebRootPath, f.FilePathFull).Replace("\\", "/"),
                DisplayName = f.FileDisplayName,
                FileDescription = f.FileDescription,
                FileType = f.FileType
            });
            
            return new JsonResult(files);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<FileViewModel> files)
        {
            IEnumerable<IFormFile> formFiles = files.Select(f => f.FormFile);
            if (files.Any())
            {
                long size = formFiles.Sum(f => f.Length);
                IEnumerable<FileContainer> savedFiles = await _fileHelper.SaveFromFormFiles(formFiles);
                if (savedFiles.Count() == files.Count())
                {
                    return Ok(new { count = formFiles.Count(), size });
                }
                else
                {
                    //TODO: Error => not all files could be uploaded
                }
            }
            return BadRequest();
        }
    }
}
