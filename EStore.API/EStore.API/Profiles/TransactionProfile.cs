using AutoMapper;
using EStore.API.DAL.Data;
using EStore.API.DTO.Transaction;
using EStore.API.Service.Models.Purchase;
using EStore.API.Service.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EStore.API.Profiles
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, TransactionModel>()
                .ReverseMap();
            CreateMap<Transaction, TransactionInsertModel>()
                .ReverseMap();
            CreateMap<PurchaseUpdateModel, TransactionInsertModel>()
                .ForMember(dest => dest.IdTransaction, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.IdPurchase, opt => opt.MapFrom(src => src.IdPurchase))
                .ForMember(dest => dest.InsertBy, opt => opt.MapFrom(src => src.UpdateBy))
                .ForMember(dest => dest.InsertDate, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<TransactionSearchSellerByerDTO, TransactionSearchSellerByerModel>()
               .ReverseMap();
            CreateMap<TransactionDTO, TransactionModel>()
               .ReverseMap();
        }
    }
}
