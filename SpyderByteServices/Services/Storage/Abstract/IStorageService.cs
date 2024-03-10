using SpyderByteResources.Responses.Abstract;

namespace SpyderByteServices.Services.Storage.Abstract
{
    public interface IStorageService
    {
        Task<IDataResponse<bool>> Upload(string fileName, Stream stream);
    }
}
