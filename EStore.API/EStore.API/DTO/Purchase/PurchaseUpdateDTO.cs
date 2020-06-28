using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EStore.API.DTO.Purchase
{
    [DisplayName("PurchaseUpdate")]
    public class PurchaseUpdateDTO
    {
        public Guid IdPurchase { get; set; }

        public int PurchaseStatus { get; set; }
    }
}
