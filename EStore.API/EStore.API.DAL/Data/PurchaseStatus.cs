using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EStore.API.DAL.Data
{
    public partial class PurchaseStatus
    {
        public PurchaseStatus()
        {
            Purchase = new HashSet<Purchase>();
        }

        [Key]
        public int IdPurchaseStatus { get; set; }
        [Required]
        [StringLength(50)]
        public string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? InsertDate { get; set; }
        public Guid? InsertBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DeleteDate { get; set; }
        public Guid? DeleteBy { get; set; }

        [InverseProperty("IdPurchaseStatusNavigation")]
        public virtual ICollection<Purchase> Purchase { get; set; }
    }
}
