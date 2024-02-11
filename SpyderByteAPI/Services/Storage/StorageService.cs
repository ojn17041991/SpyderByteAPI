using Azure.Storage.Blobs;
using SpyderByteAPI.DataAccess;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Services.Storage.Abstract;

namespace SpyderByteAPI.Services.Storage
{
    public class StorageService : IStorageService
    {
        private readonly ILogger<StorageService> logger;
        private readonly IConfiguration configuration;

        private string connectionString;
        private string containerName;

        private BlobServiceClient client;

        public StorageService(ILogger<StorageService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;

            this.connectionString = this.configuration.GetConnectionString("Storage") ?? string.Empty;
            this.containerName = this.configuration["Storage:Containers:Database"] ?? string.Empty;

            client = new BlobServiceClient(this.connectionString);
        }

        public async Task<IDataResponse<bool>> Upload(string fileName, Stream stream)
        {
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
