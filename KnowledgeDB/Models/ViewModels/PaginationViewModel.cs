using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Models.ViewModels
{
    public class PaginationInfo
    {
        public int TotalEntries { get; set; }
        public int EntriesPerPage { get; set; }
        public int CurrentPage { get; set;}
        public int TotalPages => (int)Math.Ceiling((decimal)TotalEntries / EntriesPerPage);
    }
}
