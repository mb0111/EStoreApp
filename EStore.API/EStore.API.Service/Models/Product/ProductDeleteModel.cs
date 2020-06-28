using System;

namespace EStore.API.Service.Models.Product
{
    public class ProductDeleteModel
    {
        public Guid IdProduct { get; set; }

        public bool IsActive { get; set; }

        public DateTime DeleteDate { get; set; }

        public Guid DeleteBy { get; set; }
    }
}
