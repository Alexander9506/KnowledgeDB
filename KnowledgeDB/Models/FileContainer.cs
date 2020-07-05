using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Models
{
    public class FileContainer
    {
        public int FileContainerId { get; set; }
        public string FileDisplayName { get; set; }
        public string FileType { get; set; }
        [Required]
        public string FilePathFull { get; set; }
        public string FileDescription { get; set; }
    }
}
