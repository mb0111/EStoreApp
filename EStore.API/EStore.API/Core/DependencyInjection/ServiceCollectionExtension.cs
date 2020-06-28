using EStore.API.Service.Contracts;
using EStore.API.Service.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;

namespace EStore.API.Core.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserProductService, UserProductService>();
            services.AddScoped<IPurchaseService, PurchaseService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddSingleton<IMimeMappingService>(new MimeMappingService(new FileExtensionContentTypeProvider()));

            return services;
        }
    }
}
