//TODO GitFix
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lms.MVC.Core.Entities;
using Lms.MVC.Core.Repositories;
using Lms.MVC.Data.Data;
using Lms.MVC.Data.Repositories.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Lms.MVC.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> userManager;

        public UserRepository(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
            
        }

        public async Task<ApplicationUser> GetUserWithFilesByIdAsync(string id) => await db.Users.Include(u=>u.Files).Where(u=>u.Id==id).FirstOrDefaultAsync();

        public async Task<ApplicationUser> GetUserByIdAsync(string id, bool includeCourses = false)
        {
            if (includeCourses)
            {
                return await db.Users.Include(u => u.Courses).FirstOrDefaultAsync(u => u.Id == id);
            }

            return await db.Users.FindAsync(id);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await db.Users.ToListAsync();
        }

        public string GetRole(ApplicationUser user)
        {
            var sb = new StringBuilder();
            var roles = userManager.GetRolesAsync(user).Result.ToList();

            foreach (var item in roles)
            {
                sb.AppendLine(item);
            }
            var rolesName = sb.ToString();
            return rolesName;
        }

        public void Update(ApplicationUser user)
        {
            db.Update(user);
        }

        public void Remove(ApplicationUser user)
        {
            db.Remove(user);
        }

        public bool Any(string id)
        {
            return db.Users.Any(u => u.Id == id);
        }

        public async Task ChangeRoleAsync(ApplicationUser user)
        {
            if (user is null) throw new ArgumentNullException();
            var role = user.Role;
            if (role == RoleHelper.Student && user.Courses.Count > 1 && await userManager.IsInRoleAsync(user, RoleHelper.Student))
            {
                await userManager.RemoveFromRoleAsync(user, RoleHelper.Student);
            }
            else if (role == RoleHelper.Admin)
            {
                await userManager.AddToRoleAsync(user, RoleHelper.Admin);
                await userManager.AddToRoleAsync(user, RoleHelper.Teacher);
            }
            else if (role == RoleHelper.Teacher)
            {
                await userManager.RemoveFromRoleAsync(user, RoleHelper.Student);
                await userManager.RemoveFromRoleAsync(user, RoleHelper.Admin);
                await userManager.AddToRoleAsync(user, RoleHelper.Teacher);
            }
            else if (role == RoleHelper.Student && user.Courses.Count == 1)
            {
                await userManager.AddToRoleAsync(user, RoleHelper.Student);
            }
            else
            {
                await userManager.RemoveFromRoleAsync(user, RoleHelper.Teacher);
                await userManager.RemoveFromRoleAsync(user, RoleHelper.Admin);
                await userManager.AddToRoleAsync(user, RoleHelper.Student);
            }
        }

        public async Task<ICollection<ApplicationFile>> GetAllFilesByUserId(string id)
        {
            var user = await db.Users.Include(u=>u.Files).Where(u=>u.Id==id).FirstOrDefaultAsync();
            return user.Files;
        }
    }
}