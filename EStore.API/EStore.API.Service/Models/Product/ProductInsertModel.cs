using System;

namespace EStore.API.Service.Models.Product
{
    public class ProductInsertModel : ProductModel
    {
        public DateTime InsertDate { get; set; }
        public Guid InsertBy { get; set; }
    }
}
