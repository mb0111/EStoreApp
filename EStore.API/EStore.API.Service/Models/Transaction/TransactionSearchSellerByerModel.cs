using System;
using System.Collections.Generic;
using System.Text;

namespace EStore.API.Service.Models.Transaction
{
    public class TransactionSearchSellerByerModel
    {
        public Guid IdSeller { get; set; }

        public Guid IdByer { get; set; }
    }
}
