using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteAPI.DataAccess;
using SpyderByteAPI.DataAccess.Accessors;
using SpyderByteAPI.Services.Imgur.Abstract;

namespace SpyderByteAPITest.DataAccess.JamsAccessorTests.Helper
{
    public class JamsAccessorExceptionHelper
    {
        public JamsAccessor Accessor;

        public JamsAccessorExceptionHelper()
        {
            // The DB Context is mocked out, so it will always throw an exception.
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
            var context = new Mock<ApplicationDbContext>(options);

            var logger = new Mock<ILogger<JamsAccessor>>();

            var configurationContents = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationContents)
                .Build();

            var imgurService = new Mock<IImgurService>();

            Accessor = new JamsAccessor(context.Object, logger.Object, configuration, imgurService.Object);
        }
    }
}
