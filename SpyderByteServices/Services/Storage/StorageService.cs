using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses;
using SpyderByteResources.Responses.Abstract;
using SpyderByteServices.Services.Storage.Abstract;

namespace SpyderByteServices.Services.Storage
{
    public class StorageService(ILogger<StorageService> logger, IConfiguration configuration, IAzureClientFactory<BlobServiceClient> clientFactory) : IStorageService
    {
        private readonly ILogger<StorageService> logger = logger;
        private readonly IConfiguration configuration = configuration;
        private IAzureClientFactory<BlobServiceClient> clientFactory = clientFactory;

        private string containerName = configuration["Storage:Containers:Database"] ?? string.Empty;
        private string clientName = configuration["Storage:ClientName"] ?? string.Empty;

        public async Task<IDataResponse<bool>> Upload(string fileName, Stream stream)
        {
            var client = clientFactory.CreateClient(clientName);
            var container = client.GetBlobContainerClient(containerName);
            if (!await container.ExistsAsync())
            {
                logger.LogError($"Failed to find container {containerName} in Storage Account.");
                return new DataResponse<bool>(false, ModelResult.NotFound);
            }

            stream.Position = 0;
            var blob = container.GetBlobClient(fileName);
            var response = await blob.UploadAsync(stream, true);
            if (response.GetRawResponse().IsError)
            {
                logger.LogError($"Failed to upload file {fileName} to Storage Account.");
                return new DataResponse<bool>(false, ModelResult.Error);
            }

            return new DataResponse<bool>(true, ModelResult.OK);
        }
    }
}
