using EStore.API.DAL.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EStore.API.Core.Builder
{
    public static class DataBaseBuilderExtension
    {
        /// <summary>
        /// Ensure that Database Exists
        /// </summary>
        public static void EnsureDBIsCreated(this IApplicationBuilder app)
        {
            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using var serviceScope = serviceScopeFactory.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetService<EStoreContext>();
            dbContext.Database.EnsureCreated();
        }
    }
}
