using System;
using System.ComponentModel;

namespace EStore.API.DTO.Search
{
    [DisplayName("ProductInfoSearchCriteria")]
    public class UserProductSearchCriteriaDTO : PaginationBaseDTO
    {
        public Guid? IdSeller { get; set; }

        public Guid? IdProduct { get; set; }

        public int? MinQuantity { get; set; }

        public int? MaxQuantity { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }
    }
}
