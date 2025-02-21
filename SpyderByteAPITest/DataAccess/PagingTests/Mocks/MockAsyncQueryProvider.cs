using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace SpyderByteTest.DataAccess.PagingTests.Mocks
{
    public class MockAsyncQueryProvider<T1> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _provider;

        public MockAsyncQueryProvider(IQueryProvider provider)
        {
            _provider = provider;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new MockAsyncQueryable<T1>(expression);
        }

        public IQueryable<T2> CreateQuery<T2>(Expression expression)
        {
            return new MockAsyncQueryable<T2>(expression);
        }

        public object Execute(Expression expression)
        {
            return _provider.Execute(expression)!;
        }

        public T2 Execute<T2>(Expression expression)
        {
            return _provider.Execute<T2>(expression);
        }

        public IAsyncEnumerable<T2> ExecuteAsync<T2>(Expression expression)
        {
            return new MockAsyncQueryable<T2>(expression);
        }

        T2 IAsyncQueryProvider.ExecuteAsync<T2>(Expression expression, CancellationToken cancellationToken)
        {
            return Execute<T2>(expression);
        }
    }
}
