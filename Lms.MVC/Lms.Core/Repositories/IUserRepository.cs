using System.Collections.Generic;
using System.Threading.Tasks;

using Lms.MVC.Core.Entities;

namespace Lms.MVC.Core.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();

        string GetRole(ApplicationUser user);

        Task<ApplicationUser> GetUserByIdAsync(string id, bool includeCourses);

        Task<ApplicationUser> GetUserWithFilesByIdAsync(string id);
        void Update(ApplicationUser user);

        void Remove(ApplicationUser user);

        public bool Any(string id);

        Task ChangeRoleAsync(ApplicationUser user);
        Task<ICollection<ApplicationFile>> GetAllFilesByUserId(string id);
    }
}