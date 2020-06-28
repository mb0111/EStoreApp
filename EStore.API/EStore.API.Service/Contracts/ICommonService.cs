using System.Threading.Tasks;

namespace EStore.API.Service.Contracts
{
    public interface ICommonService
    {
        Task AddAsync<TEntity>(TEntity entity) where TEntity : class;
        void Delete<TEntity>(TEntity entity) where TEntity : class;
        Task<bool> SaveChangesAsync();
    }
}
