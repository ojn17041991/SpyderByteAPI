using SpyderByteDataAccess.Models.Users;
using SpyderByteResources.Models.Responses.Abstract;

namespace SpyderByteDataAccess.Accessors.Users.Abstract
{
    public interface IUsersAccessor
    {
        Task<IDataResponse<User?>> GetAsync(Guid id);

        Task<IDataResponse<User?>> GetByUserNameAsync(string userName);

        Task<IDataResponse<User?>> PostAsync(PostUser user);

        Task<IDataResponse<User?>> PatchAsync(PatchUser user);

        Task<IDataResponse<User?>> DeleteAsync(Guid id);
    }
}
