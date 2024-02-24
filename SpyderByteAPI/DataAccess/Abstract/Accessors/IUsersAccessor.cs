using SpyderByteAPI.Models.Users;

namespace SpyderByteAPI.DataAccess.Abstract.Accessors
{
    public interface IUsersAccessor
    {
        Task<IDataResponse<User?>> GetAsync(string id);

        Task<IDataResponse<User?>> PostAsync(PostHashedUser game);

        Task<IDataResponse<User?>> DeleteAsync(string id);
    }
}
