using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteDataAccess.Accessors.Leaderboards;
using SpyderByteDataAccess.Contexts;

namespace SpyderByteTest.DataAccess.LeaderboardsAccessorTests.Helpers
{
    public class LeaderboardsAccessorExceptionHelper
    {
        public LeaderboardsAccessor Accessor;

        public LeaderboardsAccessorExceptionHelper()
        {
            // The DB Context is mocked out, so it will always throw an exception.
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
            var context = new Mock<ApplicationDbContext>(options);

            var logger = new Mock<ILogger<LeaderboardsAccessor>>();

            Accessor = new LeaderboardsAccessor(context.Object, logger.Object);
        }
    }
}
