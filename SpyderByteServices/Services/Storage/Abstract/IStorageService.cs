using SpyderByteResources.Models.Responses.Abstract;

namespace SpyderByteServices.Services.Storage.Abstract
{
    public interface IStorageService
    {
        Task<IDataResponse<bool>> UploadAsync(string fileName, Stream stream);
    }
}
