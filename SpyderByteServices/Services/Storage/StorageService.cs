using AutoMapper;
using Azure;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpyderByteResources.Enums;
using SpyderByteResources.Helpers.Encoding;
using SpyderByteResources.Models.Responses;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Models.Data;
using SpyderByteServices.Services.Storage.Abstract;

namespace SpyderByteServices.Services.Storage
{
    public class StorageService : IStorageService
    {
        private readonly ILogger<StorageService> logger;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private IAzureClientFactory<BlobServiceClient> clientFactory;

        private string containerName;
        private string clientName;

        private BlobServiceClient client;
        private BlobContainerClient container;

        public StorageService(
            ILogger<StorageService> logger,
            IConfiguration configuration,
            IMapper mapper,
            IAzureClientFactory<BlobServiceClient> clientFactory)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.mapper = mapper;
            this.clientFactory = clientFactory;

            containerName = configuration["Storage:Containers:Database"] ?? string.Empty;
            clientName = configuration["Storage:ClientName"] ?? string.Empty;

            client = clientFactory.CreateClient(clientName);
            container = client.GetBlobContainerClient(containerName);
        }

        public async Task<IDataResponse<IList<StorageFile>?>> GetFilesAsync()
        {
            if (!await container.ExistsAsync())
            {
                logger.LogError($"Failed to find container {LogEncoder.Encode(containerName)} in Storage Account.");
                return new DataResponse<IList<StorageFile>?> (null, ModelResult.NotFound);
            }

            IList<StorageFile> files = new List<StorageFile>();

            var blobs = container.GetBlobsAsync();
            await foreach (var blob in blobs)
            {
                var file = mapper.Map<StorageFile>(blob);
                files.Add(file);
            }

            return new DataResponse<IList<StorageFile>?>(files, ModelResult.OK);
        }

        public async Task<IDataResponse<bool>> UploadAsync(string fileName, Stream stream)
        {
            if (!await container.ExistsAsync())
            {
                logger.LogError($"Failed to find container {LogEncoder.Encode(containerName)} in Storage Account.");
                return new DataResponse<bool>(false, ModelResult.NotFound);
            }

            stream.Position = 0;
            var blob = container.GetBlobClient(fileName);
            var response = await blob.UploadAsync(stream, true, CancellationToken.None);
            var rawResponse = response.GetRawResponse();
            if (rawResponse.IsError)
            {
                logger.LogError($"Failed to upload file {LogEncoder.Encode(fileName)} to Storage Account.");
                return new DataResponse<bool>(false, ModelResult.Error);
            }

            return new DataResponse<bool>(true, ModelResult.OK);
        }

        public async Task<IDataResponse<bool>> DeleteAsync(StorageFile file)
        {
            if (!await container.ExistsAsync())
            {
                logger.LogError($"Failed to find container {LogEncoder.Encode(containerName)} in Storage Account.");
                return new DataResponse<bool>(false, ModelResult.NotFound);
            }

            BlobClient blob = container.GetBlobClient(file.FileName);
            if (await blob.ExistsAsync() == false)
            {
                logger.LogError($"Failed to find blob of name {file.FileName} in Storage Account.");
                return new DataResponse<bool>(false, ModelResult.NotFound);
            }

            Response response = await blob.DeleteAsync();
            if (response.IsError)
            {
                logger.LogError($"Failed to delete blob of name {file.FileName}. {response.ReasonPhrase}.");
                return new DataResponse<bool>(false, ModelResult.Error);
            }

            return new DataResponse<bool>(true, ModelResult.OK);
        }
    }
}
