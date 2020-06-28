using System.ComponentModel;

namespace EStore.API.DTO.Authenticate
{
    [DisplayName("Authenticated")]
    public class AuthenticatedDTO
    {
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}
