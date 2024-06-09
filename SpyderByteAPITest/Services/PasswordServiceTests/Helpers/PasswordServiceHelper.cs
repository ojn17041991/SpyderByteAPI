using AutoFixture;
using Microsoft.Extensions.Configuration;
using SpyderByteServices.Services.Password;

namespace SpyderByteTest.Services.PasswordServiceTests.Helpers
{
    public class PasswordServiceHelper
    {
        public PasswordService Service;

        private readonly Fixture _fixture;

        public PasswordServiceHelper()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            Dictionary<string, string?> configurationContents = new()
            {
                { "Encryption:Argon2:DegreesOfParallelism", "1" },
                { "Encryption:Argon2:MemorySize", "1024" },
                { "Encryption:Argon2:Iterations", "1" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationContents)
                .Build();

            Service = new PasswordService(configuration);
        }
    }
}
