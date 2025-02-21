using SpyderByteResources.Models.Paging.Abstract;

namespace SpyderByteResources.Models.Paging
{
    public class PagedList<T> : IPagedList<T>
    {
        public PagedList(IList<T> items, int count, int page, int pageSize)
        {
            Items = items;
            Count = count;
            Page = page;
            PageSize = pageSize;
        }

        public IList<T> Items { get; }

        public int Page { get; }

        public int PageSize { get; }

        public bool HasNextPage => Page * PageSize < Count;

        public bool HasPreviousPage => Page > 1;

        public int Count { get; }

        public bool IsReadOnly => true;

        public T this[int index]
        {
            get => Items[index];
            set => Items[index] = value;
        }
    }
}
