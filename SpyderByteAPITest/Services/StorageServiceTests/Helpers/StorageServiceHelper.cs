using AutoMapper;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteServices.Services.Storage;

namespace SpyderByteTest.Services.StorageServiceTests.Helpers
{
    public class StorageServiceHelper
    {
        public StorageService Service;

        private bool containerExists = true;
        private bool isResponseError = false;

        public StorageServiceHelper()
        {
            var logger = new Mock<ILogger<StorageService>>();

            var configuration = new Mock<IConfiguration>();

            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddProfile<SpyderByteServices.Mappers.MapperProfile>();
                }
            );
            var mapper = new Mapper(mapperConfiguration);

            var response = new Mock<Response>();
            response.Setup(x =>
                x.IsError
            ).Returns(() => {
                return isResponseError;
            });

            var azureResponse = new Mock<Response<BlobContentInfo>>();
            azureResponse.Setup(x =>
                x.GetRawResponse()
            ).Returns(() =>
            {
                return response.Object;
            });

            var blobClient = new Mock<BlobClient>();
            blobClient.Setup(x =>
                x.UploadAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            ).Returns((Stream content, bool overwrite, CancellationToken cancellationToken) =>
            {
                return Task.FromResult(azureResponse.Object);
            });

            var blobContainerClient = new Mock<BlobContainerClient>();
            blobContainerClient.Setup(x =>
                x.ExistsAsync(
                    It.IsAny<CancellationToken>()
                )
            ).Returns((CancellationToken cancellationToken) =>
            {
                return Task.FromResult(Response.FromValue(containerExists, Mock.Of<Response>()));
            });
            blobContainerClient.Setup(x =>
                x.GetBlobClient(
                    It.IsAny<string>()
                )
            ).Returns((string blobName) =>
            {
                return blobClient.Object;
            });

            var blobServiceClient = new Mock<BlobServiceClient>();
            blobServiceClient.Setup(x =>
                x.GetBlobContainerClient(
                    It.IsAny<string>()
                )
            ).Returns((string blobContainerName) =>
            {
                return blobContainerClient.Object;
            });

            var azureClientFactory = new Mock<IAzureClientFactory<BlobServiceClient>>();
            azureClientFactory.Setup(x =>
                x.CreateClient(
                    It.IsAny<string>()
                )
            ).Returns((string name) =>
            {
                return blobServiceClient.Object;
            });

            Service = new StorageService(logger.Object, configuration.Object, mapper, azureClientFactory.Object);
        }

        public void SetContainerExists(bool exists)
        {
            containerExists = exists;
        }

        public void SetIsResponseError(bool isError)
        {
            isResponseError = isError;
        }
    }
}
