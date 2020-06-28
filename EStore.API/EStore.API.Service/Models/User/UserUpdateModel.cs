using System;

namespace EStore.API.Service.Models.User
{
    public class UserUpdateModel : UserModel
    {
        public DateTime DeleteDate { get; set; }
        public Guid DeleteBy { get; set; }
    }
}
