using System;
using System.Collections.Generic;
using System.Text;

namespace EStore.API.Service.Models.Transaction
{
    public class TransactionInsertModel : TransactionModel
    {
        public DateTime InsertDate { get; set; }

        public Guid InsertBy { get; set; }
    }
}
