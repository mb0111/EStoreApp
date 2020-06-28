using System;
using System.ComponentModel;

namespace EStore.API.DTO.User
{
    [DisplayName("User")]
    public class UserDTO : UserBaseDTO
    {
        public Guid IdUser { get; set; }
    }
}
