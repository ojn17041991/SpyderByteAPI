using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Models.Users;

namespace SpyderByteAPI.Services.Users.Abstract
{
    public interface IUsersService
    {
        public Task<IDataResponse<User?>> PostAsync(PostUser user);

        public Task<IDataResponse<User?>> PatchAsync(PatchUser user);

        public Task<IDataResponse<User?>> DeleteAsync(Guid id);
    }
}
