using System;
using System.ComponentModel;

namespace EStore.API.DTO.UserProduct
{
    [DisplayName("ProductInformation")]
    public class ProductInfoDTO
    {
        public Guid IdSellerProduct { get; set; }

        public Guid IdSeller { get; set; }

        public Guid IdProduct { get; set; }

        public Guid IdCategory { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public bool IsAvailable => (Quantity > 0);
    }
}
