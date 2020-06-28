using EStore.API.Service.Models.Search;
using System.Linq;

namespace EStore.API.Service.Models.Abstraction
{
    public abstract class SearchCriteriaFilter<TEntity> : PaginationBaseModel
    {
        public abstract IQueryable<TEntity> ApplySearchCriteriaFilter(IQueryable<TEntity> source);
    }
}
