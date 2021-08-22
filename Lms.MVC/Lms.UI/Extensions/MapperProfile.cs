using AutoMapper;

using Lms.MVC.Core.Dto;
using Lms.MVC.Core.Entities;

namespace Lms.MVC.UI.Extensions
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Course, CourseDto>().ReverseMap();
            CreateMap<Module, ModuleDto>().ReverseMap();
        }
    }
}