using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Models.Data;

namespace SpyderByteServices.Services.Storage.Abstract
{
    public interface IStorageService
    {
        public string ClientPath { get; }
        
        public string ContainerPath { get; }

        public Task<IDataResponse<IList<StorageFile>?>> GetFilesAsync();

        public Task<IDataResponse<StorageFile?>> UploadAsync(string fileName, Stream stream);

        public Task<IDataResponse<StorageFile?>> DeleteAsync(string fileName);

        public Task<IDataResponse<StorageFile?>> DeleteAsync(StorageFile file);
    }
}
