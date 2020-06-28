using EStore.API.Service.Helpers;
using System;
using System.Threading.Tasks;

namespace EStore.API.Service.Contracts
{
    public interface ICategoryService
    {
        Task<EnEntityExistsStatus> CategoryExistsAsync(Guid idCategory);
    }
}
