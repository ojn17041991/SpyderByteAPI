using AutoMapper;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpyderByteServices.Services.Storage.Abstract;
using SpyderByteServices.Services.Storage.Database.Abstract;

namespace SpyderByteServices.Services.Storage.Database
{
    public class DatabaseStorageService : BaseDatabaseStorageService
    {
        public override string ClientPath => "Storage:ClientName";
        public override string ContainerPath => "Storage:Containers:Database";

        public DatabaseStorageService(
            IAzureClientFactory<BlobServiceClient> clientFactory,
            IConfiguration configuration,
            IMapper mapper,
            ILogger<BaseStorageService> logger
        ) : base(clientFactory, configuration, mapper, logger)
        { }
    }
}
