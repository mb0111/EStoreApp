using System;
using System.Collections.Generic;
using System.Text;

namespace EStore.API.Service.Models.Purchase
{
    public class PurchaseUpdateModel
    {
        public Guid IdPurchase { get; set; }

        public int IdPurchaseStatus { get; set; }

        public DateTime? UpdateDate { get; set; }

        public Guid? UpdateBy { get; set; }
    }
}
