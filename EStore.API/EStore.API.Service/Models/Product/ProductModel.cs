using EStore.API.Service.Models.FileUpload;
using System;

namespace EStore.API.Service.Models.Product
{
    public class ProductModel : FileUploadModel
    {
        public Guid IdProduct { get; set; }
        public Guid IdCategory { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool? IsActive { get; set; }
    }
}
