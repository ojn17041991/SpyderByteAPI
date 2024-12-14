using AutoMapper;
using SpyderByteResources.Models.Paging;
using SpyderByteResources.Models.Paging.Abstract;

namespace SpyderByteResources.Mappers.Converters
{
    public class PagedListConverter<TSource, TDestination> : ITypeConverter<IPagedList<TSource>, IPagedList<TDestination>>
    {
        public IPagedList<TDestination> Convert(
            IPagedList<TSource> source,
            IPagedList<TDestination> destination,
            ResolutionContext context)
        {
            return new PagedList<TDestination>(
                source.Items.Select(i => context.Mapper.Map<TDestination>(i)).ToList(),
                source.Page,
                source.PageSize);
        }
    }
}
