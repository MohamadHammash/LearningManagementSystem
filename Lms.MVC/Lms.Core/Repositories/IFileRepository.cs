using System.Collections.Generic;
using System.Threading.Tasks;

using Lms.MVC.Core.Entities;

namespace Lms.MVC.Core.Repositories
{
    public interface IFileRepository
    {
        Task<IEnumerable<ApplicationFile>> GetAllFilesAsync();
        Task<ApplicationFile> GetFileByIdAsync(int id);
        void Remove(ApplicationFile RemoveFile);

        Task AddAsync(ApplicationFile AddFile);
    }
}