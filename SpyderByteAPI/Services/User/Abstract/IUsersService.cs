using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Models.Users;

namespace SpyderByteAPI.Services.User.Abstract
{
    public interface IUsersService
    {
        public Task<IDataResponse<bool>> PostAsync(PostUser user);

        public Task<IDataResponse<bool>> DeleteAsync(string id);
    }
}
