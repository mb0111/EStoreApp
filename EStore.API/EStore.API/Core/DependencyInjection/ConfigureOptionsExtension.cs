using EStore.API.Service.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EStore.API.Core.DependencyInjection
{
    public static class ConfigureOptionsExtension
    {
        public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtConfiguration>(configuration.GetSection(nameof(JwtConfiguration)));
            services.Configure<PaginationConfiguration>(configuration.GetSection(nameof(PaginationConfiguration)));

            return services;
        }
    }
}
