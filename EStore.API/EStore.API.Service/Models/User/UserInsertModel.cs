using System;

namespace EStore.API.Service.Models.User
{
    public class UserInsertModel : UserModel
    {
        public DateTime InsertDate { get; set; }

        public Guid InsertBy { get; set; }
    }
}
