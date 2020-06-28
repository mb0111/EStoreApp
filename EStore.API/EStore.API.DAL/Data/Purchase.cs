using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EStore.API.DAL.Data
{
    public partial class Purchase
    {
        public Purchase()
        {
            Transaction = new HashSet<Transaction>();
        }

        [Key]
        public Guid IdPurchase { get; set; }
        public Guid IdUserProduct { get; set; }
        public int IdPurchaseStatus { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? InsertDate { get; set; }
        public Guid? InsertBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DeleteDate { get; set; }
        public Guid? DeleteBy { get; set; }

        [ForeignKey(nameof(IdPurchaseStatus))]
        [InverseProperty(nameof(PurchaseStatus.Purchase))]
        public virtual PurchaseStatus IdPurchaseStatusNavigation { get; set; }
        [ForeignKey(nameof(IdUserProduct))]
        [InverseProperty(nameof(UserProduct.Purchase))]
        public virtual UserProduct IdUserProductNavigation { get; set; }
        [InverseProperty("IdPurchaseNavigation")]
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}
