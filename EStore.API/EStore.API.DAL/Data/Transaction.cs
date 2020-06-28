using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EStore.API.DAL.Data
{
    public partial class Transaction
    {
        [Key]
        public Guid IdTransaction { get; set; }
        public Guid IdPurchase { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? InsertDate { get; set; }
        public Guid? InsertBy { get; set; }

        [ForeignKey(nameof(IdPurchase))]
        [InverseProperty(nameof(Purchase.Transaction))]
        public virtual Purchase IdPurchaseNavigation { get; set; }
    }
}
