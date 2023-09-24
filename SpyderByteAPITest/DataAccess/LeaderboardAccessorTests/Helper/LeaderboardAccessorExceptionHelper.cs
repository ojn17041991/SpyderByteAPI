using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteAPI.DataAccess.Accessors;
using SpyderByteAPI.DataAccess;

namespace SpyderByteAPITest.DataAccess.LeaderboardAccessorTests.Helper
{
    public class LeaderboardAccessorExceptionHelper
    {
        public LeaderboardAccessor Accessor;

        public LeaderboardAccessorExceptionHelper()
        {
            // The DB Context is mocked out, so it will always throw an exception.
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
            var context = new Mock<ApplicationDbContext>(options);

            var logger = new Mock<ILogger<LeaderboardAccessor>>();

            Accessor = new LeaderboardAccessor(context.Object, logger.Object);
        }
    }
}
