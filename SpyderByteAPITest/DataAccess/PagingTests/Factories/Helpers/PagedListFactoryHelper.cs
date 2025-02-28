using Microsoft.EntityFrameworkCore;
using Moq;
using SpyderByteDataAccess.Contexts;
using SpyderByteResources.Paging.Factories;

namespace SpyderByteTest.DataAccess.PagingTests.Factories.Helpers
{
    public class PagedListFactoryHelper<T>
    {
        public PagedListFactory Factory;
        public ApplicationDbContext Context;

        public PagedListFactoryHelper()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            Context = new ApplicationDbContext(options);

            Factory = new PagedListFactory();
        }
    }
}
