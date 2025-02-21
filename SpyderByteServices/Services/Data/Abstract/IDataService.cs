using SpyderByteResources.Models.Responses.Abstract;

namespace SpyderByteServices.Services.Data.Abstract
{
    public interface IDataService
    {
        Task<IDataResponse<bool>> Backup();
    }
}