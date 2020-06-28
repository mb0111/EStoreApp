using System;

namespace EStore.API.Service.Models.Product
{
    public class ProductUpdateModel : ProductModel
    {
        public DateTime UpdateDate { get; set; }
        public Guid UpdateBy { get; set; }
    }
}
