using Microsoft.EntityFrameworkCore;
using SpyderByteDataAccess.Paging.Factories.Abstract;
using SpyderByteResources.Models.Paging;
using SpyderByteResources.Models.Paging.Abstract;
using System.Linq.Expressions;

namespace SpyderByteResources.Paging.Factories
{
    public class PagedListFactory : IPagedListFactory
    {
        public async Task<IPagedList<T>> BuildAsync<T>(
            IQueryable<T> query,
            Expression<Func<T, bool>> filteringFunction,
            Expression<Func<T, object>> orderingFunction,
            string? direction,
            int page,
            int pageSize)
        {
            // Get the total number of items before filtering.
            int count = query.Count();

            // Apply the filtering.
            query = query.Where(filteringFunction);

            // Apply the ordering function with direction.
            if (direction != null && direction.ToLower() == "desc")
            {
                query = query.OrderByDescending(orderingFunction);
            }
            else
            {
                query = query.OrderBy(orderingFunction);
            }

            // Apply paging and convert to list.
            IList<T> items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Construct the list.
            return new PagedList<T>(items, count, page, pageSize);
        }
    }
}
