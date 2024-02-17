using SpyderByteAPI.DataAccess.Abstract;

namespace SpyderByteAPI.Services.Storage.Abstract
{
    public interface IStorageService
    {
        Task<IDataResponse<bool>> Upload(string fileName, Stream stream);
    }
}
