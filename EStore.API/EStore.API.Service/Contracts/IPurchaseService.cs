using EStore.API.Service.Helpers;
using EStore.API.Service.Models.Purchase;
using System;
using System.Threading.Tasks;

namespace EStore.API.Service.Contracts
{
    public interface IPurchaseService : ICommonService
    {
        Task<EnEntityExistsStatus> PurchaseExistsAsync(Guid idPurchase);

        Task<Output<EnPurchaseStatus>> GetPurchaseStatusAsync(Guid idPurchase);

        Task<Output<PurchaseModel>> GetPurchaseAsync(Guid idPurchase);

        Task<Output<PurchaseModel>> CreatePurchaseAsync(PurchaseInsertModel purchaseInsertModel);

        Task<Output<PurchaseModel>> UpdatePurchaseAsync(PurchaseUpdateModel purchaseUpdateModel);
    }
}
