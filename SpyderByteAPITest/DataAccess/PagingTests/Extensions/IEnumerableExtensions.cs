using SpyderByteTest.DataAccess.PagingTests.Mocks;

namespace SpyderByteTest.DataAccess.PagingTests.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IQueryable<T> ToAsyncQueryable<T>(this IEnumerable<T> source)
        {
            return new MockAsyncQueryable<T>(source ?? throw new ArgumentNullException(nameof(source)));
        }
    }
}
