using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Models.Users;

namespace SpyderByteServices.Services.Users.Abstract
{
    public interface IUsersService
    {
        public Task<IDataResponse<User?>> GetAsync(Guid id);

        public Task<IDataResponse<User?>> PostAsync(PostUser user);

        public Task<IDataResponse<User?>> PatchAsync(PatchUser user);

        public Task<IDataResponse<User?>> DeleteAsync(Guid id);
    }
}
