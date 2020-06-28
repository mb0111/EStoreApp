using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EStore.API.DTO.User
{
    [DisplayName("UserBase")]
    public class UserBaseDTO
    {
        [StringLength(250)]
        public string FirstName { get; set; }

        [StringLength(250)]
        public string LastName { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        public bool? IsActive { get; set; }
    }
}
