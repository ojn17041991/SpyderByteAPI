using System.Linq.Expressions;

namespace SpyderByteTest.DataAccess.PagingTests.Mocks
{

    public class MockAsyncQueryable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public MockAsyncQueryable(IEnumerable<T> enumerable) : base(enumerable)
        {

        }

        public MockAsyncQueryable(Expression expression) : base(expression)
        {

        }

        public IAsyncEnumerator<T> GetEnumerator()
        {
            return new MockAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new MockAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new MockAsyncQueryProvider<T>(this);

    }
}
