using System;

namespace EStore.API.Service.Models.User
{
    public class UserDeleteModel : UserModel
    {
        public DateTime DeleteDate { get; set; }
        public Guid DeleteBy { get; set; }
    }
}
