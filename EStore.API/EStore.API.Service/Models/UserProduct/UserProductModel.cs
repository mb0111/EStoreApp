using System;

namespace EStore.API.Service.Models.UserProduct
{
    public class UserProductModel
    {
        public Guid IdUserProduct { get; set; }
        public Guid IdUser { get; set; }
        public Guid IdProduct { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }
}
