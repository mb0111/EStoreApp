using System;

namespace EStore.API.Service.Models.Purchase
{
    public class PurchaseInsertModel : PurchaseModel
    {
        public DateTime InsertDate { get; set; }
        public Guid InsertBy { get; set; }
    }
}
