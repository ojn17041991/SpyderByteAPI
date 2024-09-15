using Microsoft.Extensions.Configuration;
using SpyderByteServices.Services.Encoding;

namespace SpyderByteTest.Services.EncodingServiceTests.Helpers
{
    public class EncodingServiceHelper
    {
        public EncodingService Service;

        public EncodingServiceHelper()
        {
            var configurationSettings = new Dictionary<string, string?>
            {
                { "Authentication:EncodingKey", Guid.NewGuid().ToString() },
                { "Authentication:TimeoutMinutes", "1" },
                { "Authentication:Issuer", "Test" },
                { "Authentication:Audience", "Test" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationSettings)
                .Build();

            Service = new EncodingService(configuration);
        }
    }
}
