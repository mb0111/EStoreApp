using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EStore.API.DAL.Data
{
    public partial class Product
    {
        public Product()
        {
            UserProduct = new HashSet<UserProduct>();
        }

        [Key]
        public Guid IdProduct { get; set; }
        public Guid IdCategory { get; set; }
        [Required]
        [StringLength(250)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        [Required]
        public bool? IsActive { get; set; }
        public byte[] ImageFileData { get; set; }
        [StringLength(250)]
        public string ImageFileName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? InsertDate { get; set; }
        public Guid? InsertBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DeleteDate { get; set; }
        public Guid? DeleteBy { get; set; }

        [ForeignKey(nameof(IdCategory))]
        [InverseProperty(nameof(Category.Product))]
        public virtual Category IdCategoryNavigation { get; set; }
        [InverseProperty("IdProductNavigation")]
        public virtual ICollection<UserProduct> UserProduct { get; set; }
    }
}
