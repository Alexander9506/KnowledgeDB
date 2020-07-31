using KnowledgeDB.Models.Context;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Models.Repositories
{
    public class FileRepository : IFileRepository
    {
        private EFContext context;
        private readonly ILogger _logger;
        public FileRepository(EFContext context, ILogger<FileRepository> logger)
        {
            this.context = context;
            _logger = logger;
        }

        public IQueryable<FileContainer> FileContainers => context.FileContainers;

        public async Task<bool> DeleteFileContainer(FileContainer container)
        {
            if (container != null)
            {
                try
                {
                    context.Remove(container);
                    await context.SaveChangesAsync();
                    return true;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Could'nt delete FileContainer");
                }
            }
            return false;
        }

        public async Task<FileContainer> SaveFileContainer(FileContainer container)
        {
            try
            {
                context.Update(container);
                if (await context.SaveChangesAsync() > 0)
                {
                    return container;
                }
            }catch(Exception e)
            {
                _logger.LogError(e, "Could'nt save FileContainer");
            }
            return null;
        }
    }
}
