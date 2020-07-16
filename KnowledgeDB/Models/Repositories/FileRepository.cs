using KnowledgeDB.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeDB.Models.Repositories
{
    public class FileRepository : IFileRepository
    {
        private EFContext context;

        public FileRepository(EFContext context)
        {
            this.context = context;
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
                    //TODO: Logging
                }
            }
            return false;
        }

        public async Task<bool> SaveFileContainer(FileContainer container)
        {
            try
            {
                context.Update(container);
                await context.SaveChangesAsync();
            }catch(Exception e)
            {
                //TODO: Logging
                return false;
            }
            
            return true;
        }
    }
}
