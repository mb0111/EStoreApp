using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EStore.API.DAL.Data
{
    public partial class User
    {
        public User()
        {
            UserProduct = new HashSet<UserProduct>();
            UserRole = new HashSet<UserRole>();
        }

        [Key]
        public Guid IdUser { get; set; }
        [StringLength(250)]
        public string FirstName { get; set; }
        [StringLength(250)]
        public string LastName { get; set; }
        [Required]
        [StringLength(250)]
        public string UserName { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
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

        [InverseProperty("IdUserNavigation")]
        public virtual ICollection<UserProduct> UserProduct { get; set; }
        [InverseProperty("IdUserNavigation")]
        public virtual ICollection<UserRole> UserRole { get; set; }
    }
}
