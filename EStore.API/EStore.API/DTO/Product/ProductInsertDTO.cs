using System;
using System.ComponentModel;

namespace EStore.API.DTO.Product
{
    [DisplayName("ProductInsert")]
    public class ProductInsertDTO : ProductDTO
    {
        public Guid? IdProduct { get; set; }
    }
}
