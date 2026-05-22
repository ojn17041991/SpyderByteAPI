using AutoFixture;
using AutoMapper;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteServices.Models.Data;
using SpyderByteServices.Services.Storage.Abstract;
using SpyderByteTest.Services.StorageServiceTests.Enums;
using SpyderByteTest.Services.StorageServiceTests.Mocks;

namespace SpyderByteTest.Services.StorageServiceTests.Helpers
{
    public class StorageServiceHelper
    {
        public BaseStorageService Service;

        private Fixture fixture;
        private Mock<IAzureClientFactory<BlobServiceClient>> azureClientFactory;
        private Mock<IConfiguration> configuration;
        private IMapper mapper;
        private Mock<ILogger<BaseStorageService>> logger;

        private IDictionary<StorageFunction, bool> azureSuccessFlags = new Dictionary<StorageFunction, bool>();
        private bool containerExists = true;
        private bool blobExists = true;

        private IList<BlobItem> blobs = new List<BlobItem>();
        private BlobItem? nextBlobForDeletion = null!;

        public StorageServiceHelper()
        {
            fixture = new Fixture();

            var uploadResponse = new Mock<Response>();
            uploadResponse.Setup(x =>
                x.IsError
            ).Returns(() => {
                return azureSuccessFlags[StorageFunction.UploadAsync];
            });

            var deleteResponse = new Mock<Response>();
            deleteResponse.Setup(x =>
                x.IsError
            ).Returns(() => {
                return azureSuccessFlags[StorageFunction.DeleteAsync];
            });

            var existsResponse = new Mock<Response>();
            existsResponse.Setup(x =>
                x.IsError
            ).Returns(() =>
            {
                return azureSuccessFlags[StorageFunction.ExistsAsync];
            });

            var getPropertiesResponse = new Mock<Response>();
            getPropertiesResponse.Setup(x =>
                x.IsError
            ).Returns(() =>
            {
                return azureSuccessFlags[StorageFunction.GetPropertiesAsync];
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
                return Task.FromResult(Response.FromValue<BlobContentInfo>(null!, uploadResponse.Object));
            });
            blobClient.Setup(x =>
                x.DeleteAsync(
                    It.IsAny<DeleteSnapshotsOption>(),
                    It.IsAny<BlobRequestConditions>(),
                    It.IsAny<CancellationToken>()
                )
            ).Returns((DeleteSnapshotsOption deleteSnapshotsOption, BlobRequestConditions blobRequestConditions, CancellationToken cancellationToken) =>
            {
                // There's no way of knowing which blob is being deleted, so the only way to know is to ask the unit test to...
                // ...let the helper know in advance which blob it is attempting to delete, so it can be deleted when this function is hit.
                blobs.Remove(nextBlobForDeletion);
                return Task.FromResult(deleteResponse.Object);
            });
            blobClient.Setup(x =>
                x.ExistsAsync(
                    It.IsAny<CancellationToken>()
                )
            ).Returns((CancellationToken cancellationToken) =>
            {
                return Task.FromResult(Response.FromValue<bool>(blobExists, existsResponse.Object));
            });
            blobClient.Setup(x =>
                x.GetPropertiesAsync(
                    It.IsAny<BlobRequestConditions>(),
                    It.IsAny<CancellationToken>()
                )
            ).Returns((BlobRequestConditions blobRequestConditions, CancellationToken cancellationToken) =>
            {
                return Task.FromResult(Response.FromValue<BlobProperties>(fixture.Create<BlobProperties>(), getPropertiesResponse.Object));
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
            blobContainerClient.Setup(x =>
                x.GetBlobsAsync(
                    It.IsAny<BlobTraits>(),
                    It.IsAny<BlobStates>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
            )).Returns(() =>
            {
                return AsyncPageable<BlobItem>
                    .FromPages
                    (
                        new Page<BlobItem>[]
                        {
                            Page<BlobItem>.FromValues(blobs.ToArray(), null, Mock.Of<Response>())
                        }
                    );
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

            azureClientFactory = new Mock<IAzureClientFactory<BlobServiceClient>>();
            azureClientFactory.Setup(x =>
                x.CreateClient(
                    It.IsAny<string>()
                )
            ).Returns((string name) =>
            {
                return blobServiceClient.Object;
            });

            configuration = new Mock<IConfiguration>();

            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddProfile<SpyderByteServices.Mappers.MapperProfile>();
                }
            );
            mapper = new Mapper(mapperConfiguration);

            logger = new Mock<ILogger<BaseStorageService>>();

            Service = new MockStorageService(azureClientFactory.Object, configuration.Object, mapper, logger.Object);
        }

        public void SetContainerExists(bool exists)
        {
            containerExists = exists;
        }

        public void SetBlobExists(bool exists)
        {
            blobExists = exists;
        }

        public void SetIsResponseError(StorageFunction function, bool isError)
        {
            azureSuccessFlags[function] = isError;
        }

        public IList<BlobItem> GetBlobs()
        {
            return blobs.Select(b => b).ToList();
        }

        public BlobItem AddBlob(string name)
        {
            BlobItemProperties properties = BlobsModelFactory.BlobItemProperties(false, createdOn: DateTime.UtcNow);
            BlobItem blob = BlobsModelFactory.BlobItem(name: name, properties: properties);
            blobs.Add(blob);
            return blob;
        }

        public void PrepareNextBlobForDeletion(BlobItem blob)
        {
            nextBlobForDeletion = blob;
        }

        public StorageFile ConvertBlobToStorageFile(BlobItem blob)
        {
            return mapper.Map<StorageFile>(blob);
        }
    }
}
