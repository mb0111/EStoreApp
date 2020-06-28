using System;

namespace EStore.API.Service.Models.UserProduct
{
    public class UserProductInsertModel : UserProductModel
    {
        public DateTime InsertDate { get; set; }
        public Guid InsertBy { get; set; }
    }
}
