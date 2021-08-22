using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Lms.MVC.Core.Entities;
using Lms.MVC.Core.Repositories;
using Lms.MVC.Data.Data;

using Microsoft.EntityFrameworkCore;

namespace Lms.MVC.Data.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly ApplicationDbContext db;

        public FileRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<ApplicationFile>> GetAllFilesAsync()
        {
            return await db.Files.ToListAsync();
        }

        public async Task<ApplicationFile> GetFileByIdAsync(int id)
        {
            return await db.Files.Where(f => f.Id == id).FirstOrDefaultAsync();
        }

        public void Remove(ApplicationFile RemoveFile)
        {
            db.Files.Remove(RemoveFile);
        }

        public async Task AddAsync(ApplicationFile AddFile)
        {
            await db.Files.AddAsync(AddFile);
        }
    }
}