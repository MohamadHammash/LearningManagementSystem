using AutoMapper;

namespace Lms.MVC.Data.Data
{
    public class LmsMVCDataMapperProfile : Profile
    {
        public LmsMVCDataMapperProfile()
        {
            //CreateMap<ApplicationUser, ApplicationUsersListViewModel>()
            //    .ForMember(
            //    dest => dest.Email,
            //    from => from.MapFrom(u => u.Email))
            //    .ForMember(
            //    dest => dest.Role,
            //    opt => opt.MapFrom<RoleResolver>());
        }
    }

    //public class RoleResolver : IValueResolver<ApplicationUser, ApplicationUsersListViewModel, string>
    //{
    //    private readonly IUoW uow;

    // public RoleResolver(IUoW uow) { this.uow = uow; } public string Resolve(ApplicationUser
    // source, ApplicationUsersListViewModel destination, string destMember, ResolutionContext
    // context) { var role = uow.UserRepository.GetRole(source);

    //        return role;
    //    }
    //}
}