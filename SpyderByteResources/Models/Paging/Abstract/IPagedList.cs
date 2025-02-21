namespace SpyderByteResources.Models.Paging.Abstract
{
    public interface IPagedList<T>
    {
        public IList<T> Items { get; }

        public int Count { get; }

        public int Page { get; }

        public int PageSize { get; }

        public bool HasNextPage { get; }

        public bool HasPreviousPage { get; }
    }
}
