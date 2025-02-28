namespace SpyderByteTest.DataAccess.PagingTests.Mocks
{
    public class MockAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public MockAsyncEnumerator(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public void Dispose()
        {
            _enumerator.Dispose();
        }

        public T Current => _enumerator.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_enumerator.MoveNext());
        }

#pragma warning disable CS1998

        public async ValueTask DisposeAsync()
        {
            _enumerator.Dispose();
        }

#pragma warning restore CS1998
    }
}
