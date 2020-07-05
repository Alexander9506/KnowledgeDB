using Microsoft.AspNetCore.Http;
using System.IO;

namespace KnowledgeDB.Models
{
    public static class FileContainerFactory
    {
        public static FileContainer CreateFileContainer(IFormFile formFile, string path, string fileName)
        {
            FileContainer container = new FileContainer
            {
                FileDisplayName = formFile.Name,
                FilePathFull = Path.Combine(path, fileName),
                FileType = formFile.ContentType
            };

            return container;
        }
    }
}
