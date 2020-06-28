using AutoMapper;
using EStore.API.DAL.Data;
using EStore.API.DTO;
using EStore.API.DTO.Authenticate;
using EStore.API.DTO.User;
using EStore.API.Service.Models.Authenticate;
using EStore.API.Service.Models.User;
using System;

namespace EStore.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<AuthenticateDTO, AuthenticateModel>()
                .ReverseMap();
            CreateMap<AuthenticatedDTO, AuthenticatedModel>()
                .ReverseMap();
            CreateMap<UserDTO, UserInsertModel>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom((src, dest) => src.IsActive.HasValue ? src.IsActive : true))
                .ForMember(dest => dest.InsertDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ReverseMap();
            CreateMap<UserInsertDTO, UserInsertModel>()
                .ForMember(dest => dest.IdUser, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom((src, dest) => src.IsActive.HasValue ? src.IsActive : true))
                .ForMember(dest => dest.InsertDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ReverseMap();
            CreateMap<UserModel, UserDTO>()
                .ReverseMap();
            CreateMap<User, AuthenticatedModel>()
                .ReverseMap();
            CreateMap<User, UserInsertModel>()
                .ReverseMap();
            CreateMap<User, UserModel>()
                .ReverseMap();
            CreateMap<User, UserDeleteModel>()
                .ReverseMap()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => false));
        }
    }
}
