using System.Threading.Tasks;

namespace Lms.MVC.Core.Repositories
{
    public interface IUoW
    {
        ICourseRepository CourseRepository { get; }

        IModuleRepository ModuleRepository { get; }

        IUserRepository UserRepository { get; }

        IActivityRepository ActivityRepository { get; }

        IPublicationRepository PublicationRepository { get; }

        IFileRepository FileRepository { get; }

        Task CompleteAsync();
    }
}