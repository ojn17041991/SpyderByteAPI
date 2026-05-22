using AutoMapper;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpyderByteResources.Enums;
using SpyderByteResources.Helpers.Encoding;
using SpyderByteResources.Models.Responses;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Models.Data;

namespace SpyderByteServices.Services.Storage.Abstract
{
    public abstract class BaseStorageService : IStorageService
    {
        private readonly IAzureClientFactory<BlobServiceClient> clientFactory;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly ILogger<BaseStorageService> logger;

        private readonly string clientName;
        private readonly BlobServiceClient client;
        private readonly string containerName;
        private readonly BlobContainerClient container;

        public abstract string ClientPath { get; }
        public abstract string ContainerPath { get; }

        public BaseStorageService(
            IAzureClientFactory<BlobServiceClient> clientFactory,
            IConfiguration configuration,
            IMapper mapper,
            ILogger<BaseStorageService> logger
        )
        {
            this.clientFactory = clientFactory;
            this.configuration = configuration;
            this.mapper = mapper;
            this.logger = logger;

            clientName = configuration[ClientPath] ?? string.Empty;
            client = clientFactory.CreateClient(this.clientName);

            containerName = configuration[ContainerPath] ?? string.Empty;
            container = client.GetBlobContainerClient(this.containerName);
        }

        public virtual async Task<IDataResponse<IList<StorageFile>?>> GetFilesAsync()
        {
            if (!await container.ExistsAsync())
            {
                logger.LogError($"Failed to find container {LogEncoder.Encode(containerName)} in Storage Account.");
                return new DataResponse<IList<StorageFile>?>(null, ModelResult.NotFound);
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

        public virtual async Task<IDataResponse<StorageFile?>> UploadAsync(string fileName, Stream stream)
        {
            if (!await container.ExistsAsync())
            {
                logger.LogError($"Failed to find container {LogEncoder.Encode(containerName)} in Storage Account.");
                return new DataResponse<StorageFile?>(null, ModelResult.NotFound);
            }

            stream.Position = 0;
            var client = container.GetBlobClient(fileName);
            var response = await client.UploadAsync(stream, true, CancellationToken.None);
            var rawResponse = response.GetRawResponse();
            if (rawResponse.IsError)
            {
                logger.LogError($"Failed to upload file {LogEncoder.Encode(fileName)} to Storage Account.");
                return new DataResponse<StorageFile?>(null, ModelResult.Error);
            }

            var properties = await client.GetPropertiesAsync();
            var storageFile = mapper.Map<StorageFile>(client, options =>
            {
                options.Items["properties"] = properties.Value;
            });

            return new DataResponse<StorageFile?>(storageFile, ModelResult.Created);
        }

        public virtual async Task<IDataResponse<StorageFile?>> DeleteAsync(string fileName)
        {
            return await deleteAsync(fileName);
        }

        public virtual async Task<IDataResponse<StorageFile?>> DeleteAsync(StorageFile file)
        {
            return await deleteAsync(file.FileName);
        }

        #region Private

        private async Task<IDataResponse<StorageFile?>> deleteAsync(string fileName)
        {
            if (!await container.ExistsAsync())
            {
                logger.LogError($"Failed to find container {LogEncoder.Encode(containerName)} in Storage Account.");
                return new DataResponse<StorageFile?>(null, ModelResult.NotFound);
            }

            var client = container.GetBlobClient(Path.GetFileName(fileName));
            if (await client.ExistsAsync() == false)
            {
                logger.LogError($"Failed to find blob of name {fileName} in Storage Account.");
                return new DataResponse<StorageFile?>(null, ModelResult.NotFound);
            }

            var properties = await client.GetPropertiesAsync();

            var response = await client.DeleteAsync();
            if (response.IsError)
            {
                logger.LogError($"Failed to delete blob of name {fileName}. {response.ReasonPhrase}.");
                return new DataResponse<StorageFile?>(null, ModelResult.Error);
            }

            var storageFile = mapper.Map<StorageFile>(client, options =>
            {
                options.Items["properties"] = properties.Value;
            });

            return new DataResponse<StorageFile?>(storageFile, ModelResult.OK);
        }

        #endregion
    }
}
