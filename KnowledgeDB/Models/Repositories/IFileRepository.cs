using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Models.Repositories
{
    public interface IFileRepository
    {
        IQueryable<FileContainer> FileContainers { get;}

        Task<bool> SaveFileContainer(FileContainer container);
        Task<bool> DeleteFileContainer(FileContainer container);
    }
}
