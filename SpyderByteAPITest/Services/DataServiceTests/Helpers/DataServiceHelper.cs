using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteResources.Enums;
using SpyderByteResources.Models.Responses;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Services.Data;
using SpyderByteServices.Services.Storage.Abstract;

namespace SpyderByteTest.Services.DataServiceTests.Helpers
{
    public class DataServiceHelper
    {
        public DataService Service;

        private const string DATABASE_FOLDER_NAME = "Databases";

        private Mock<ILogger<DataService>> _logger;
        private IConfiguration _configuration;
        private Mock<IStorageService> _storageService;

        private bool allowStorageServiceSuccess = true;

        public DataServiceHelper()
        {
            _configuration = new Mock<IConfiguration>().Object;

            _logger = new Mock<ILogger<DataService>>();

            _storageService = new Mock<IStorageService>();
            _storageService.Setup(s =>
                s.UploadAsync(
                    It.IsAny<string>(),
                    It.IsAny<Stream>()
            )).Returns((string fileName, Stream stream) =>
                Task.FromResult(
                    (
                        allowStorageServiceSuccess
                        ?
                            new DataResponse<bool>(
                                true,
                                ModelResult.OK
                            )
                        :
                            new DataResponse<bool>(
                                false,
                                ModelResult.Error
                            )
                    )
                    as IDataResponse<bool>
                )
            );

            Service = new DataService(_logger.Object, _configuration, _storageService.Object);
        }

        public void BuildFullConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string?>>
                {
                    new KeyValuePair<string, string?>("Database:BackupFileExtension", ".backup"),
                })
                .Build();

            Service = new DataService(_logger.Object, _configuration, _storageService.Object);
        }

        public void BuildDatabaseDirectory()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), DATABASE_FOLDER_NAME);
            Directory.CreateDirectory(filePath);
        }

        public void ClearDatabaseDirectory()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), DATABASE_FOLDER_NAME);
            if (Directory.Exists(filePath))
            {
                Directory.Delete(filePath, true);
            }
        }

        public void BuildDatabaseFile()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), DATABASE_FOLDER_NAME, ".db");
            var fileStream = File.Create(filePath);
            fileStream.Close();
        }

        public void ClearDatabaseFile()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), DATABASE_FOLDER_NAME, ".db");
            File.Delete(filePath);
        }

        public void SetStorageServiceResponse(bool allowSuccess)
        {
            allowStorageServiceSuccess = allowSuccess;
        }
    }
}
