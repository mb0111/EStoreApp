using EStore.API.Service.Helpers;
using EStore.API.Service.Models.Product;
using System;
using System.Threading.Tasks;

namespace EStore.API.Service.Contracts
{
    public interface IProductService : ICommonService
    {
        Task<EnEntityExistsStatus> ProductExistsAsync(Guid idProduct);

        Task<Output<ProductModel>> GetProductAsync(Guid idProduct);

        Task<Output<ProductImageModel>> GetProductImageAsync(Guid idProduct);

        Task<Output<ProductModel>> CreateProductAsync(ProductInsertModel productInsertModel);

        Task<Output<ProductModel>> UpdateProductAsync(ProductUpdateModel productUpdateModel);

        Task<Output<dynamic>> DeleteProductAsync(ProductDeleteModel productDeleteModel);
    }
}
