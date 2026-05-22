using AutoMapper;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpyderByteServices.Services.Storage.Abstract;

namespace SpyderByteServices.Services.Storage.Database.Abstract
{
    public abstract class BaseDatabaseStorageService : BaseStorageService
    {
        public BaseDatabaseStorageService(
            IAzureClientFactory<BlobServiceClient> clientFactory,
            IConfiguration configuration,
            IMapper mapper,
            ILogger<BaseStorageService> logger
        ) : base(clientFactory, configuration, mapper, logger)
        {
        }
    }
}
