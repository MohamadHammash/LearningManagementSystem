using System.Collections.Generic;
using System.Threading.Tasks;

using Lms.MVC.Core.Entities;

namespace Lms.MVC.Core.Repositories
{
    public interface IActivityRepository
    {
        Task AddAsync(Activity added);

        void Remove(Activity removed);

        Task<IEnumerable<Activity>> GetAllActivitiesAsync();

        Task<Activity> GetActivityWithFilesAsync(int? id);
        Task<Activity> GetActivityAsync(int? id, bool includeActivityType);
        Task<IEnumerable<int>> GetAllLateAssignmentsFromModuleAsync(int id, string userId);
        Task<IEnumerable<int>> GetAllLateAssignmentsFromCourseAsync(int courseId, string userId);
        Task<bool> SaveAsync();
        Task<IEnumerable<Activity>> GetAllActivitiesFromModuleAsync(int id);
        int GetNextDueAssignment(int? courseId, int? moduleId);
        Task<bool> ActivityExists(int id);

        void Update(Activity activity);

        Task<IEnumerable<ActivityType>> GetAllActivityTypesAsync();
        Task<ICollection<ApplicationFile>> GetAllFilesByActivityId(int id);
        Task<List<Activity>> GetAllActivitiesByModuleIdAsync(int id);
    }
}