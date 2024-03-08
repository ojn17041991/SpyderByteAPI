using SpyderByteAPI.Models.Users;

namespace SpyderByteAPI.DataAccess.Abstract.Accessors
{
    public interface IUsersAccessor
    {
        Task<IDataResponse<User?>> GetAsync(Guid id);

        Task<IDataResponse<User?>> GetByUserNameAsync(string userName);

        Task<IDataResponse<User?>> PostAsync(PostHashedUser user);

        Task<IDataResponse<User?>> PatchAsync(PatchUser user);

        Task<IDataResponse<User?>> DeleteAsync(Guid id);
    }
}
