using System;

namespace KnowledgeDB.Models
{
    public interface IChangeInformationBase
    {
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
