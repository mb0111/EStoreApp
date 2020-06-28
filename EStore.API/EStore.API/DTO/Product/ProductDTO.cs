using EStore.API.DTO.FileUpload;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EStore.API.DTO.Product
{
    [DisplayName("Product")]
    public class ProductDTO : FileUploadDTO
    {
        [Required]
        public Guid IdCategory { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(0, 9999999999999999.99)]
        public decimal Price { get; set; }

        public bool? IsActive { get; set; }
    }
}
