using AutoMapper;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpyderByteServices.Services.Storage.Abstract;
using SpyderByteServices.Services.Storage.Image.Abstract;

namespace SpyderByteServices.Services.Storage.Image
{
    public class ImageStorageService : BaseImageStorageService
    {
        public override string ClientPath => "Storage:ClientName";
        public override string ContainerPath => "Storage:Containers:Image";

        public ImageStorageService(
            IAzureClientFactory<BlobServiceClient> clientFactory,
            IConfiguration configuration,
            IMapper mapper,
            ILogger<BaseStorageService> logger
        ) : base(clientFactory, configuration, mapper, logger)
        { }
    }
}
