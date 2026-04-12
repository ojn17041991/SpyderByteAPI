using AutoMapper;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpyderByteServices.Services.Storage.Abstract;

namespace SpyderByteTest.Services.StorageServiceTests.Mocks
{
    public class MockStorageService : BaseStorageService
    {
        public MockStorageService(
            IAzureClientFactory<BlobServiceClient> clientFactory,
            IConfiguration configuration,
            IMapper mapper,
            ILogger<BaseStorageService> logger
        ) : base(clientFactory, configuration, mapper, logger)
        {
        }

        public override string ClientPath => string.Empty;

        public override string ContainerPath => string.Empty;
    }
}
