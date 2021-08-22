using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Lms.MVC.Core.Entities;

namespace Lms.MVC.Core.Repositories
{
    public interface ICourseRepository
    {
        Task AddAsync(Course added);

        Task<IEnumerable<Course>> GetAllCoursesAsync(bool includeModules = false, bool includeUsers = false);

        Task<Course> GetCourseWithFilesAsync(int? id);
        Task<Course> GetCourseAsync(int? id, bool includeModules = false, bool includeUsers = false);

        void Remove(Course removed);

        Task<bool> SaveAsync();

        Task<bool> CourseExists(int id);

        void Update(Course course);

        Task<DateTime> CalculateEndDateAsync(int id);

        void SetAllCoursesEndDate();

        Task<Course> SetCourseEndDateAsync(int id);
        Task<ICollection<ApplicationFile>> GetAllFilesByCourseId(int id);
        IEnumerable<ApplicationUser> GetTeachers(int? courseId);
        IEnumerable<ApplicationUser> GetTeachersByModule(int? moduleId);
    }
}