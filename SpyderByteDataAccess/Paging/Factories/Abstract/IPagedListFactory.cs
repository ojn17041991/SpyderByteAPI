using SpyderByteResources.Models.Paging.Abstract;
using System.Linq.Expressions;

namespace SpyderByteDataAccess.Paging.Factories.Abstract
{
    public interface IPagedListFactory
    {
        Task<IPagedList<T>> BuildAsync<T>(
            IQueryable<T> query,
            Expression<Func<T, bool>> filteringFunction,
            Expression<Func<T, object>> orderingFunction,
            string? direction,
            int page,
            int pageSize
        );
    }
}
