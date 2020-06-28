using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EStore.API.DAL.Data
{
    public partial class UserRole
    {
        [Key]
        public Guid IdUserRole { get; set; }
        public Guid IdUser { get; set; }
        public int IdRole { get; set; }
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

        [ForeignKey(nameof(IdRole))]
        [InverseProperty(nameof(Role.UserRole))]
        public virtual Role IdRoleNavigation { get; set; }
        [ForeignKey(nameof(IdUser))]
        [InverseProperty(nameof(User.UserRole))]
        public virtual User IdUserNavigation { get; set; }
    }
}
