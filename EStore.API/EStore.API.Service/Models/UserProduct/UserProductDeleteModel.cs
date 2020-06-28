using System;
using System.Collections.Generic;
using System.Text;

namespace EStore.API.Service.Models.UserProduct
{
    public class UserProductDeleteModel
    {
        public bool IsActive { get; set; }

        public DateTime DeleteDate { get; set; }

        public Guid DeleteBy { get; set; }
    }
}
