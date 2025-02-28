using AutoFixture;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteResources.Enums;
using SpyderByteResources.Models.Responses;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Models.Data;
using SpyderByteServices.Services.Data;
using SpyderByteServices.Services.Storage.Abstract;
using SpyderByteTest.Services.DataServiceTests.Enums;

namespace SpyderByteTest.Services.DataServiceTests.Helpers
{
    public class DataServiceHelper
    {
        public DataService Service;

        private const string DATABASE_FOLDER_NAME = "Databases";

        private IList<StorageFile> _storageFiles;
        private IDictionary<DataFunction, bool> _storageServiceSuccessFlags;
        private int _retentionPeriod = 1;

        private Fixture _fixture;

        private Mock<ILogger<DataService>> _logger;
        private IConfiguration _configuration;
        private Mock<IStorageService> _storageService;

        public DataServiceHelper()
        {
            _storageFiles = new List<StorageFile>();

            _storageServiceSuccessFlags = new Dictionary<DataFunction, bool>
            {
                {
                    DataFunction.GetFilesAsync, true
                },
                {
                    DataFunction.UploadAsync, true
                },
                {
                    DataFunction.DeleteAsync, true
                },
            };

            _fixture = new Fixture();

            _logger = new Mock<ILogger<DataService>>();

            _configuration = new Mock<IConfiguration>().Object;

            _storageService = new Mock<IStorageService>();
            _storageService.Setup(s =>
                s.GetFilesAsync()
            ).Returns(() =>
                Task.FromResult(
                    (
                        _storageServiceSuccessFlags[DataFunction.GetFilesAsync]
                        ?
                            new DataResponse<IList<StorageFile>?>(
                                _storageFiles.Select(f => f).ToList(),
                                ModelResult.OK
                            )
                        :
                            new DataResponse<IList<StorageFile>?>(
                                null,
                                ModelResult.Error
                            )
                    )
                    as IDataResponse<IList<StorageFile>?>
                )
            );
            _storageService.Setup(s =>
                s.UploadAsync(
                    It.IsAny<string>(),
                    It.IsAny<Stream>()
            )).Returns((string fileName, Stream stream) =>
                Task.FromResult(
                    (
                        _storageServiceSuccessFlags[DataFunction.UploadAsync]
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
            _storageService.Setup(s =>
                s.DeleteAsync(
                    It.IsAny<StorageFile>()
            )).Returns((StorageFile file) =>
                {
                    if (_storageServiceSuccessFlags[DataFunction.DeleteAsync])
                    {
                        _storageFiles.Remove(file);
                        return Task.FromResult(
                            new DataResponse<bool>(
                                true,
                                ModelResult.OK
                            )
                            as IDataResponse<bool>
                        );
                    }
                    else
                    {
                        return Task.FromResult(
                            new DataResponse<bool>(
                                false,
                                ModelResult.Error
                            )
                            as IDataResponse<bool>
                        );
                    }
                }
            );

            Service = new DataService(_logger.Object, _configuration, _storageService.Object);
        }

        public void BuildFullConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string?>>
                {
                    new KeyValuePair<string, string?>("Database:BackupFileExtension", ".backup"),
                    new KeyValuePair<string, string?>("Database:NumberOfHoursToRetainBackup", _retentionPeriod.ToString()),
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

        public void SetStorageServiceResponse(DataFunction function, bool allowSuccess)
        {
            _storageServiceSuccessFlags[function] = allowSuccess;
        }

        public void SetRetentionPeriod(int retentionPeriod)
        {
            _retentionPeriod = retentionPeriod;

            BuildFullConfiguration();
        }

        public IList<StorageFile> GetStorageFiles()
        {
            return _storageFiles;
        }

        public StorageFile GenerateStorageFile()
        {
            return _fixture.Create<StorageFile>();
        }

        public void AddFileToStorage(StorageFile file)
        {
            _storageFiles.Add(file);
        }
    }
}
