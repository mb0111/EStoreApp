using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EStore.API.DTO.Authenticate
{
    [DisplayName("Authenticate")]
    public class AuthenticateDTO
    {
        [Required]
        [Display(Name = "Username")]
        [StringLength(250, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string Username { get; set; }

        [Required]
        [RegularExpression(@"[a-zA-Z0-9._@+-]{6,100}", ErrorMessage = "The {0} must be 6 to 100 valid characters which are any digit, any letter and -._@+.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
