using AutoMapper;
using EStore.API.DAL.Data;
using EStore.API.DTO.Product;
using EStore.API.Service.Models.Product;
using EStore.API.Service.Models.UserProduct;
using System;

namespace EStore.API.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductDTO, ProductModel>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom((src, dest) => src.IsActive.HasValue ? src.IsActive : true))
                .ReverseMap();
            CreateMap<ProductInsertDTO, ProductInsertModel>()
                .ForMember(dest => dest.IdProduct, opt => opt.MapFrom((src, dest) => src.IdProduct.HasValue ? src.IdProduct : Guid.NewGuid()))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom((src, dest) => src.IsActive.HasValue ? src.IsActive : true))
                .ForMember(dest => dest.InsertDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ReverseMap();
            CreateMap<ProductUpdateDTO, ProductUpdateModel>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom((src, dest) => src.IsActive.HasValue ? src.IsActive : true))
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ReverseMap();
            CreateMap<ProductInsertModel, Product>()
                .ReverseMap();
            CreateMap<ProductModel, Product>()
                .ReverseMap();
            CreateMap<ProductImageModel, Product>()
                .ReverseMap();
            CreateMap<ProductDeleteModel, Product>()
                .ReverseMap();
            CreateMap<ProductDeleteModel, UserProductDeleteModel>()
                .ReverseMap();
        }
    }
}
