using SpyderByteAPI.DataAccess.Abstract;

namespace SpyderByteAPI.Services.Data.Abstract
{
    public interface IDataService
    {
        Task<IDataResponse<bool>> Backup();
    }
}