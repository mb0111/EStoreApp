using EStore.API.Service.Models.Configuration;
using System.ComponentModel;

namespace EStore.API.DTO.Search
{
    [DisplayName("PaginationBase")]
    public class PaginationBaseDTO
    {
        private const int DefaultPageNumber = 0b1;
        private const int DefaultPageSize = 0b1100100;
        public static PaginationBaseDTO EvaluatePaginationWithConfig(PaginationBaseDTO paginationBase,
                                                                      PaginationConfiguration paginationConfiguration)
        {
            if (!paginationBase.PageNumber.HasValue)
            {
                paginationBase.PageNumber = paginationConfiguration?.PageNumber ?? DefaultPageNumber;
            }
            if (!paginationBase.PageSize.HasValue)
            {
                paginationBase.PageSize = paginationConfiguration?.PageSize ?? paginationConfiguration?.MaxPageSize ?? DefaultPageSize;
            }
            else if (paginationConfiguration.MaxPageSize.HasValue && paginationBase.PageSize.Value > paginationConfiguration.MaxPageSize)
            {
                paginationBase.PageSize = paginationConfiguration.MaxPageSize;
            }
            return paginationBase;
        }

        private int? pageNumber;
        private int? pageSize;
        private string orderBy;

        public int? PageNumber
        {
            get
            {
                return pageNumber;
            }
            set
            {
                pageNumber = value;
            }
        }
        public int? PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = value;
            }
        }

    }
}
