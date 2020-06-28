using EStore.API.Service.Helpers;
using EStore.API.Service.Models.Common;
using EStore.API.Service.Models.Search;
using EStore.API.Service.Models.UserProduct;
using System;
using System.Threading.Tasks;

namespace EStore.API.Service.Contracts
{
    public interface IUserProductService : ICommonService
    {
        Task<EnEntityExistsStatus> UserProductExistsAsync(Guid idUserProduct);

        Task<Output<UserProductModel>> GetUserProductAsync(Guid idUserProduct);

        Task<Output<PagedList<ProductInfoModel>>> SearchUserProductAsync(UserProductSearchCriteria searchCriteria);

        Task<Output<UserProductModel>> CreateUserProductAsync(UserProductInsertModel userProductInsertModel);

    }
}
