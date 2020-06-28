using System;
using System.Collections.Generic;
using System.Text;

namespace EStore.API.Service.Models.Transaction
{
    public class TransactionModel
    {
        public Guid IdTransaction { get; set; }

        public Guid IdPurchase { get; set; }

        public decimal Amount { get; set; }
    }
}
