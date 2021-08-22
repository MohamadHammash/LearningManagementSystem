using System;

using AutoMapper;

using Lms.API.Core.Dto;
using Lms.API.Core.Entities;

namespace Lms.API.Data.Data
{
    public class LmsAPIDataMapperProfile : Profile
    {
        public LmsAPIDataMapperProfile()
        {
            CreateMap<Author, AuthorDto>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom((src => DateTime.Now.Year - src.DateOfBirth.Year)))
                .ReverseMap();
        }
    }
}