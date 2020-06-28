using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EStore.API.DAL.Data
{
    public partial class Category
    {
        public Category()
        {
            Product = new HashSet<Product>();
        }

        [Key]
        public Guid IdCategory { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
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

        [InverseProperty("IdCategoryNavigation")]
        public virtual ICollection<Product> Product { get; set; }
    }
}
