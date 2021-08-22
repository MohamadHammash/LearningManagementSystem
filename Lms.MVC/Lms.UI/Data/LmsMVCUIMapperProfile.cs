using AutoMapper;

using Lms.API.Core.Entities;
using Lms.MVC.Core.Entities;
using Lms.MVC.Core.Repositories;
using Lms.MVC.UI.Models.ViewModels;
using Lms.MVC.UI.Models.ViewModels.ActivityViewModels;
using Lms.MVC.UI.Models.ViewModels.ApplicationUserViewModels;
using Lms.MVC.UI.Models.ViewModels.CourseViewModels;
using Lms.MVC.UI.Models.ViewModels.ModuleViewModels;
using Lms.MVC.UI.Models.ViewModels.PublicationViewModels;

namespace Lms.MVC.UI
{
    public class LmsMVCUIMapperProfile : Profile
    {
        public LmsMVCUIMapperProfile()
        {
            // Courses
            CreateMap<Course, CreateCourseViewModel>().ReverseMap();
            CreateMap<Course, ListCourseViewModel>().ReverseMap();
            CreateMap<Course, DetailCourseViewModel>();
            CreateMap<Course, EditCourseViewModel>().ReverseMap()
                .ForMember(c => c.Modules, from => from.Ignore());

            // Modules
            CreateMap<Module, CreateModuleViewModel>().ReverseMap();
            CreateMap<ListModuleViewModel, Module>().ReverseMap();
            CreateMap<Module, EditModuleViewModel>().ReverseMap();

            // Activities
            CreateMap<Activity, CreateActivityViewModel>().ReverseMap();
            CreateMap<Activity, ListActivityViewModel>().ReverseMap();
            CreateMap<Activity, DetailActivityViewModel>().ReverseMap();
            CreateMap<Activity, EditActivityViewModel>().ReverseMap()
                .ForMember(a => a.ModuleId,
                opt => opt.Ignore());

            // activity calendar
            CreateMap<Activity, SchedulerEvent>()
                .ForMember(dest => dest.start_date, opt => opt.MapFrom(src => src.StartDate))
                    .ForMember(dest => dest.end_date, opt => opt.MapFrom(src => src.EndDate))
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                    .ForMember(dest => dest.text, opt => opt.MapFrom(src => src.Description));

            CreateMap<ApplicationUser, DetailsApplicationUserViewModel>();
            CreateMap<ApplicationUser, DeleteApplicationUserViewModel>().ReverseMap();

            CreateMap<ApplicationUser, ListApplicationUsersViewModel>()
              .ForMember(
              dest => dest.Email,
              from => from.MapFrom(u => u.Email))
              .ForMember(
              dest => dest.Role,
              opt => opt.MapFrom<RoleResolver>());

            CreateMap<ApplicationUser, EditApplicationUserViewModel>();

            //.ForMember(u => u.Courses, act => act.Ignore());
            CreateMap<EditApplicationUserViewModel, ApplicationUser>()
                                                                .ForMember(u => u.Courses, act => act.Ignore());

            //Publications/Literature
            CreateMap<CreatePublicationViewModel, Publication>()
              .ForMember(dest => dest.Authors, opt => opt.Ignore())
              .ForMember(dest => dest.Subject, opt => opt.Ignore());
        }
    }

    public class RoleResolver : IValueResolver<ApplicationUser, ListApplicationUsersViewModel, string>
    {
        private readonly IUoW uow;

        public RoleResolver(IUoW uow)
        {
            this.uow = uow;
        }

        public string Resolve(ApplicationUser source, ListApplicationUsersViewModel destination, string destMember, ResolutionContext context)
        {
            var role = uow.UserRepository.GetRole(source);

            return role;
        }
    }
}