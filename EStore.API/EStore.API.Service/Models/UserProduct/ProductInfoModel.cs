using System;

namespace EStore.API.Service.Models.UserProduct
{
    public class ProductInfoModel
    {
        public Guid IdUserProduct { get; set; }

        public Guid IdUser { get; set; }

        public Guid IdProduct { get; set; }

        public Guid IdCategory { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
