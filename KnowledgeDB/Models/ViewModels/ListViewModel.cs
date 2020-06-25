using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Models.ViewModels
{
    public class ListViewModel
    {
        public PaginationInfo Pagination { get; set; }
        public IEnumerable<Object> Entries { get; set; }
        public string PartialViewName { get; set; }
    }
}
