using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EStore.API.Service.Models.Common
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPrevious => (CurrentPage > 1);
        public bool HasNext => (CurrentPage < TotalPages);

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int? pageNumber, int? pageSize)
        {
            int count = await source.CountAsync();
            int _pageNumber = 1;
            int _pageSize = count;
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                _pageNumber = pageNumber.Value;
                _pageSize = pageSize.Value;
                source.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }
            var items = await source.ToListAsync();
            return new PagedList<T>(items, count, _pageNumber, _pageSize);
        }
    }
}
