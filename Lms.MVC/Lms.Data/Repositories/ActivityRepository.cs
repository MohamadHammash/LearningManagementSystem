using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Lms.MVC.Core.Entities;
using Lms.MVC.Core.Repositories;
using Lms.MVC.Data.Data;

using Microsoft.EntityFrameworkCore;

namespace Lms.MVC.Data.Repositories
{
    internal class ActivityRepository : IActivityRepository
    {
        private readonly ApplicationDbContext db;

        public ActivityRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<Activity> GetActivityWithFilesAsync(int? id) => await db.Activities.Include(c => c.Files).Where(c => c.Id == id).FirstOrDefaultAsync();

        public async Task AddAsync(Activity added) => await db.AddAsync(added);

        public void Remove(Activity removed) => db.Remove(removed);

        public async Task<IEnumerable<Activity>> GetAllActivitiesAsync() => await db.Activities.ToListAsync();

        public async Task<IEnumerable<Activity>> GetAllActivitiesFromModuleAsync(int id) => await db.Activities.Where(a => a.ModuleId == id).ToListAsync();

        public async Task<IEnumerable<int>> GetAllLateAssignmentsFromModuleAsync(int id, string userId)
        {
            // Get all assignments for current module
            var allLateAssignments = await db.Activities.Where(a => a.ModuleId == id).Where(a => a.EndDate < DateTime.Now)
            .Where(m => m.ActivityType.Name == "Assignment").ToListAsync();

            var currentUserFiles = await db.Users.Include(i => i.Files).Where(u => u.Id == userId).SelectMany(f => f.Files).ToListAsync();

            var lateAssignments = new List<int>();
            if (currentUserFiles is null || allLateAssignments is null)
            {
                return null;
            }
            if (currentUserFiles.Count == 0)
            {
                return allLateAssignments.Select(f => f.Id);
            }
            foreach (var assignment in allLateAssignments)
            {
                if (assignment.Files == null)
                {
                    lateAssignments.Add(assignment.Id);
                }
                else
                {
                    bool Late = true;
                    foreach (var file in currentUserFiles)
                    {
                        if (assignment.Files.Contains(file))
                        {
                            Late = false;
                            break;
                        }
                    }
                    if (Late)
                    {
                        lateAssignments.Add(assignment.Id);
                    }
                }
            }
            return lateAssignments.Distinct();
        }

        public async Task<IEnumerable<int>> GetAllLateAssignmentsFromCourseAsync(int courseId, string userId)
        {
            var allLateAssignments = db.Courses
                .Include(c => c.Modules).ThenInclude(m => m.Activities).ThenInclude(a => a.ActivityType)
                .FirstOrDefault(c => c.Id == courseId).Modules
                .SelectMany(m => m.Activities)
                .Where(a => a.EndDate < DateTime.Now && a.ActivityType.Name.ToLower() == "assignment");

            var currentUserFiles = await db.Users.Include(i => i.Files).Where(u => u.Id == userId).SelectMany(f => f.Files).ToListAsync();

            var lateAssignments = new List<int>();
            if (currentUserFiles is null || allLateAssignments is null)
            {
                return null;
            }
            if (currentUserFiles.Count == 0)
            {
                return allLateAssignments.Select(f => f.Id);
            }

            foreach (var assignment in allLateAssignments)
            {
                if (assignment.Files == null)
                {
                    lateAssignments.Add(assignment.Id);
                }
                else
                {
                    bool Late = true;
                    foreach (var file in currentUserFiles)
                    {
                        if (assignment.Files.Contains(file))
                        {
                            Late = false;
                            break;
                        }
                    }
                    if (Late)
                    {
                        lateAssignments.Add(assignment.Id);
                    }
                }
            }
            return lateAssignments.Distinct();
        }

        public async Task<Activity> GetActivityAsync(int? id, bool includeActivityType)
        {
            if (includeActivityType)
            {
                return await db.Activities.Include(f => f.ActivityType).FirstOrDefaultAsync(c => c.Id == id);
            }
            else
            {
                return await db.Activities.FirstOrDefaultAsync(c => c.Id == id);
            }
        }

        public async Task<bool> SaveAsync() => (await db.SaveChangesAsync()) >= 0;

        public async Task<bool> ActivityExists(int id) => await db.Activities.AnyAsync(c => c.Id == id);

        public void Update(Activity activity) => db.Update(activity);

        public async Task<IEnumerable<ActivityType>> GetAllActivityTypesAsync() => await db.ActivityTypes.ToListAsync();

        public async Task<ICollection<ApplicationFile>> GetAllFilesByActivityId(int id)
        {
            var activity = await db.Activities.Where(c => c.Id == id).Include(c => c.Files).FirstOrDefaultAsync();
            return activity.Files;
        }

        public async Task<List<Activity>> GetAllActivitiesByModuleIdAsync(int id)
        {
            return await db.Activities.Where(a => a.ModuleId == id).ToListAsync();
        }

        public int GetNextDueAssignment(int? courseId, int? moduleId)
        {
            if (!(moduleId == null))
            {
                var assignment = db.Activities

                    // .Include(a=>a.ActivityType)
                    .Where(a => a.ModuleId == (int)moduleId && a.ActivityType.Name.ToLower() == "assignment")
                    .FirstOrDefault(a => a.StartDate <= DateTime.Now && a.EndDate > DateTime.Now);
                if (assignment is null)
                {
                    return -1;
                }
                else return assignment.Id;
            }
            else if (!(courseId == null))
            {
                var assignment = db.Courses

                    // .Include(c=>c.Modules).ThenInclude(m=>m.Activities).ThenInclude(a=>a.ActivityType)
                    .FirstOrDefault(c => c.Id == (int)courseId).Modules.SelectMany(m => m.Activities)
                    .Where(a => a.ActivityType.Name.ToLower() == "assignment")
                    .FirstOrDefault(a => a.StartDate <= DateTime.Now && a.EndDate > DateTime.Now);

                if (assignment is null)
                {
                    return -1;
                }
                else return assignment.Id;
            }
            else
            {
                return -1;
            }
        }
    }
}