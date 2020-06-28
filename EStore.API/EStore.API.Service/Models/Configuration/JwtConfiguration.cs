namespace EStore.API.Service.Models.Configuration
{
    public class JwtConfiguration
    {
        public string Secret { get; set; }
        public int ExpireTimeInSeconds { get; set; }
    }
}
