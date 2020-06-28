using System;

namespace EStore.API.Service.Models.User
{
    public class UserModel
    {
        public Guid IdUser { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public bool? IsActive { get; set; }
    }
}
