using System;
using System.ComponentModel;

namespace EStore.API.DTO.Purchase
{
    [DisplayName("Purchase")]
    public class PurchaseDTO
    {
        public Guid IdPurchase { get; set; }

        public int PurchaseStatus { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal TotalAmount => (Price * Quantity);
    }
}
