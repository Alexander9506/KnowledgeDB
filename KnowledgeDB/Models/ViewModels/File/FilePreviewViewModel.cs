using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Models.ViewModels.File
{
    public class FilePreviewViewModel
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string FileUrl { get; set; }
        public string FileDescription { get; set; }
    }
}
