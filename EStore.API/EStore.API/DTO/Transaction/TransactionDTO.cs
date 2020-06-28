using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EStore.API.DTO.Transaction
{
    [DisplayName("Transaction")]
    public class TransactionDTO
    {
        public Guid IdTransaction { get; set; }

        public Guid IdPurchase { get; set; }

        public decimal Amount { get; set; }
    }
}
