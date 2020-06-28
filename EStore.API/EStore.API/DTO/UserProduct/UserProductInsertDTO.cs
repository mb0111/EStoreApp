using EStore.API.DTO.Product;
using System;
using System.ComponentModel;

namespace EStore.API.DTO.UserProduct
{
    [DisplayName("ProductInfoInsert")]
    public class UserProductInsertDTO : ProductDTO
    {
        public Guid? IdUser { get; set; }

        public int Quantity { get; set; }

    }
}
