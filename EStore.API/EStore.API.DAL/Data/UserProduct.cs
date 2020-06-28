using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EStore.API.DAL.Data
{
    public partial class UserProduct
    {
        public UserProduct()
        {
            Purchase = new HashSet<Purchase>();
        }

        [Key]
        public Guid IdUserProduct { get; set; }
        public Guid IdUser { get; set; }
        public Guid IdProduct { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        [Required]
        public bool? IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? InsertDate { get; set; }
        public Guid? InsertBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DeleteDate { get; set; }
        public Guid? DeleteBy { get; set; }

        [ForeignKey(nameof(IdProduct))]
        [InverseProperty(nameof(Product.UserProduct))]
        public virtual Product IdProductNavigation { get; set; }
        [ForeignKey(nameof(IdUser))]
        [InverseProperty(nameof(User.UserProduct))]
        public virtual User IdUserNavigation { get; set; }
        [InverseProperty("IdUserProductNavigation")]
        public virtual ICollection<Purchase> Purchase { get; set; }
    }
}
