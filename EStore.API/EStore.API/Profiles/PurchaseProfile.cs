using AutoMapper;
using EStore.API.DAL.Data;
using EStore.API.DTO.Purchase;
using EStore.API.Service.Models.Purchase;
using System;

namespace EStore.API.Profiles
{
    public class PurchaseProfile : Profile
    {
        public PurchaseProfile()
        {
            CreateMap<PurchaseInsertDTO, PurchaseInsertModel>()
                .ForMember(dest => dest.IdPurchase, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.IdUserProduct, opt => opt.MapFrom(src => src.IdSellerProduct))
                .ForMember(dest => dest.InsertDate, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<PurchaseInsertModel, Purchase>()
                .ReverseMap();
            CreateMap<Purchase, PurchaseModel>()
                .ReverseMap();
            CreateMap<PurchaseModel, PurchaseDTO>()
                .ForMember(dest => dest.PurchaseStatus, opt => opt.MapFrom(src => src.IdPurchaseStatus))
                .ReverseMap();
            CreateMap<PurchaseUpdateDTO, PurchaseUpdateModel>()
                .ForMember(dest => dest.IdPurchaseStatus, opt => opt.MapFrom(src => src.PurchaseStatus))
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<Purchase, PurchaseUpdateModel>()
                .ReverseMap();
        }
    }
}
