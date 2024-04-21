using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteDataAccess.Accessors.Users;
using SpyderByteDataAccess.Contexts;

namespace SpyderByteTest.DataAccess.UsersAccessorTests.Helpers
{
    public class UsersAccessorExceptionHelper
    {
        public UsersAccessor Accessor;

        public UsersAccessorExceptionHelper()
        {
            // The DB Context is mocked out, so it will always throw an exception.
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
            var context = new Mock<ApplicationDbContext>(options);

            var logger = new Mock<ILogger<UsersAccessor>>();

            Accessor = new UsersAccessor(context.Object, logger.Object);
        }
    }
}
