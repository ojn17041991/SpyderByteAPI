using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.EntityFrameworkCore;
using SpyderByteDataAccess.Contexts;
using SpyderByteDataAccess.Accessors.Games;

namespace SpyderByteTest.DataAccess.GamesAccessorTests.Helpers
{
    public class GamesAccessorExceptionHelper
    {
        public GamesAccessor Accessor;

        public GamesAccessorExceptionHelper()
        {
            // The DB Context is mocked out, so it will always throw an exception.
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
            var context = new Mock<ApplicationDbContext>(options);

            var logger = new Mock<ILogger<GamesAccessor>>();

            var configurationContents = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationContents)
                .Build();

            Accessor = new GamesAccessor(context.Object, logger.Object, configuration);
        }
    }
}
