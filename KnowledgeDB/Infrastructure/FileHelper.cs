using KnowledgeDB.Models;
using KnowledgeDB.Models.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Infrastructure
{

    public interface IFileHelper
    {
        Task<bool> DeleteFileAsync(FileContainer container);
        Task<IEnumerable<FileContainer>> SaveFromFormFiles(IEnumerable<IFormFile> files, String basePath = null);
    }

    public class FileHelper : IFileHelper
    {
        private readonly IFileRepository _fileRepository;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger _logger;

        public FileHelper(
            IFileRepository fileRepository, 
            IConfiguration configuration, 
            IWebHostEnvironment environment,
            ILogger<FileHelper> logger)
        {
            _fileRepository = fileRepository;
            _configuration = configuration;
            _environment = environment;
            _logger = logger;
        }

        public async Task<bool> DeleteFileAsync(FileContainer container)
        {
            if(container != null && container.FileContainerId > 0)
            {
                try
                {
                    if (File.Exists(container.FilePathFull))
                    {
                        File.Delete(container.FilePathFull);
                    }

                    return await _fileRepository.DeleteFileContainer(container);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "File could'nt be deleted correctly");
                    return false;
                }
            }

            return true;
        }

        public async Task<IEnumerable<FileContainer>> SaveFromFormFiles(IEnumerable<IFormFile> files, String basePath = null)
        {
            List<FileContainer> savedContainer = new List<FileContainer>();

            if (String.IsNullOrWhiteSpace(basePath))
            {
                basePath = Path.Combine(_environment.WebRootPath, _configuration.GetValue<String>("ImagePath"));
            }
            String[] allowedFileExtensions = _configuration.GetSection("AllowedFileExtensions").Get<String[]>();

            foreach (var file in files)
            {
                //New random Filename
                string originalFileExtension = Path.GetExtension(file.FileName).Replace(".", string.Empty);
                string newFileName = Path.GetRandomFileName();
                if (allowedFileExtensions.Contains(originalFileExtension.ToLower()))
                {
                    if (newFileName.Contains("."))
                    {
                        newFileName = newFileName.Substring(0, newFileName.IndexOf(".") - 1);
                        newFileName += "." + originalFileExtension;
                    }
                }

                //Create and Save FileContainer
                FileContainer container = FileContainerFactory.CreateFileContainer(file, basePath, newFileName);

                if (container != null)
                {
                    try
                    {
                        using (var stream = System.IO.File.Create(container.FilePathFull))
                        {
                            await file.CopyToAsync(stream);
                        }

                        savedContainer.Add(await _fileRepository.SaveFileContainer(container));
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "File could'nt be saved correctly");
                    }
                }
            }
            return savedContainer;
        }
    }
}
