using AutoMapper;
using EStore.API.DTO.Header;
using EStore.API.Service.Models.Common;
using EStore.API.Service.Models.UserProduct;

namespace EStore.API.Profiles
{
    public class PaginationMetadataProfile : Profile
    {
        public PaginationMetadataProfile()
        {
            CreateMap<PagedList<ProductInfoModel>, PaginationMetadataDTO>();
        }
    }
}
