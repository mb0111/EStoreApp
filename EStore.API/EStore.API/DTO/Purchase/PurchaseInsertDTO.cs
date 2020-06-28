using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EStore.API.DTO.Purchase
{
    [DisplayName("PurchaseInsert")]
    public class PurchaseInsertDTO
    {
        [Required]
        public Guid IdSellerProduct { get; set; }

        [Required]
        [DefaultValue(1)]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0, 9999999999999999.99)]
        public decimal Price { get; set; }
    }
}
