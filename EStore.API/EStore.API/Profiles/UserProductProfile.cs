using AutoMapper;
using EStore.API.DAL.Data;
using EStore.API.DTO.Search;
using EStore.API.DTO.UserProduct;
using EStore.API.Service.Models.Product;
using EStore.API.Service.Models.Search;
using EStore.API.Service.Models.UserProduct;
using System;

namespace EStore.API.Profiles
{
    public class UserProductProfile : Profile
    {
        public UserProductProfile()
        {
            CreateMap<UserProductDTO, UserProductModel>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom((src, dest) => src.IsActive.HasValue ? src.IsActive : true))
                .ReverseMap();
            CreateMap<UserProductInsertDTO, UserProductInsertModel>()
                .ForMember(dest => dest.IdUserProduct, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom((src, dest) => src.IsActive.HasValue ? src.IsActive : true))
                .ForMember(dest => dest.InsertDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ReverseMap();
            CreateMap<UserProductInsertDTO, ProductInsertModel>()
                .ForMember(dest => dest.IdProduct, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom((src, dest) => src.IsActive.HasValue ? src.IsActive : true))
                .ForMember(dest => dest.InsertDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ReverseMap();
            CreateMap<UserProductInsertModel, UserProduct>()
                .ReverseMap();
            CreateMap<UserProduct, UserProductModel>()
                .ReverseMap();
            CreateMap<UserProductSearchCriteriaDTO, UserProductSearchCriteria>()
                .ForMember(dest => dest.IdUser, opt => opt.MapFrom(src => src.IdSeller))
                .ReverseMap();
            CreateMap<PaginationBaseDTO, UserProductSearchCriteriaDTO>()
                .ReverseMap();
            CreateMap<UserProduct, ProductInfoModel>()
                .ForMember(dest => dest.IdCategory, opt => opt.MapFrom(src => src.IdProductNavigation.IdCategory))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.IdProductNavigation.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.IdProductNavigation.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));
            CreateMap<ProductInfoModel, ProductInfoDTO>()
                .ForMember(dest => dest.IdSellerProduct, opt => opt.MapFrom(src => src.IdUserProduct))
                .ForMember(dest => dest.IdSeller, opt => opt.MapFrom(src => src.IdUser));
            CreateMap<UserProduct, UserProductDeleteModel>()
                .ReverseMap();
        }
    }
}
