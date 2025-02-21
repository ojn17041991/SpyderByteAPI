﻿using Microsoft.EntityFrameworkCore;
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

            // Apply filtering and ordering.
            IList<T> filteredItems = await query.ToListAsync();

            // Get the total number of items before paging.
            int count = filteredItems.Count();

            // Page the results.
            IList<T> pagedItems = (IList<T>)filteredItems
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            // Construct the list.
            return new PagedList<T>(pagedItems, count, page, pageSize);
        }
    }
}
