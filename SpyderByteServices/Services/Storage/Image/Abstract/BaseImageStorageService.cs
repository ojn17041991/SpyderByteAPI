using AutoMapper;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpyderByteServices.Services.Storage.Abstract;

namespace SpyderByteServices.Services.Storage.Image.Abstract
{
    public abstract class BaseImageStorageService : BaseStorageService
    {
        public BaseImageStorageService(
            IAzureClientFactory<BlobServiceClient> clientFactory,
            IConfiguration configuration,
            IMapper mapper,
            ILogger<BaseStorageService> logger
        ) : base(clientFactory, configuration, mapper, logger)
        {
        }
    }
}
