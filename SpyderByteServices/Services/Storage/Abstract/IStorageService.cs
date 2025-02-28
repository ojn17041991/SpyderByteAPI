using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Models.Data;

namespace SpyderByteServices.Services.Storage.Abstract
{
    public interface IStorageService
    {
        Task<IDataResponse<IList<StorageFile>?>> GetFilesAsync();

        Task<IDataResponse<bool>> UploadAsync(string fileName, Stream stream);

        Task<IDataResponse<bool>> DeleteAsync(StorageFile file);
    }
}
