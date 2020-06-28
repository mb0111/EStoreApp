using System;
using System.ComponentModel;

namespace EStore.API.DTO.Base
{
    [DisplayName("Base")]
    public class BaseDTO
    {
        public Guid IdUser { get; set; } = Guid.Empty;
    }
}
