using EStore.API.DTO.User;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EStore.API.DTO
{
    [DisplayName("Register")]
    public class UserInsertDTO : UserBaseDTO
    {
        [Required]
        [RegularExpression(@"[a-zA-Z0-9._@+-]{6,100}", ErrorMessage = "The {0} must be 6 to 100 valid characters which are any digit, any letter and -._@+.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
