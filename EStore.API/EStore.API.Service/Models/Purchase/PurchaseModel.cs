using System;

namespace EStore.API.Service.Models.Purchase
{
    public class PurchaseModel
    {
        public Guid IdPurchase { get; set; }

        public Guid IdUserProduct { get; set; }

        public int IdPurchaseStatus { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
