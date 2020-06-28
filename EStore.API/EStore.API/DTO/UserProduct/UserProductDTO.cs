using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EStore.API.DTO.UserProduct
{
    [DisplayName("ProductInfo")]
    public class UserProductDTO
    {
        [Required]
        public Guid IdUser { get; set; }

        [Required]
        public Guid IdProduct { get; set; }

        [DefaultValue(0)]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0, 9999999999999999.99)]
        public decimal Price { get; set; }

        public bool? IsActive { get; set; }
    }
}
