using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Models.ViewModels.File
{
    public class FileViewModel
    {
        public IFormFile FormFile { get; set; }
        public String GUIID { get; set; }
    }
}
