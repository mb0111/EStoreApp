using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EStore.API.DTO.Product
{
    [DisplayName("ProductUpdate")]
    public class ProductUpdateDTO : ProductDTO
    {
        [Required]
        public Guid IdProduct { get; set; }
    }
}
