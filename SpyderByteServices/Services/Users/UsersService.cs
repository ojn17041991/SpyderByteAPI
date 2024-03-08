using SpyderByteAPI.DataAccess;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.DataAccess.Abstract.Accessors;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Helpers.Authentication;
using SpyderByteAPI.Models.Users;
using SpyderByteAPI.Services.Users.Abstract;

namespace SpyderByteAPI.Services.Users
{
    public class UsersService : IUsersService
    {
        private readonly IUsersAccessor usersAccessor;
        private readonly ILogger<UsersService> logger;
        private readonly PasswordHasher passwordHasher;

        public UsersService(IUsersAccessor usersAccessor, ILogger<UsersService> logger, PasswordHasher passwordHasher)
        {
            this.usersAccessor = usersAccessor;
            this.logger = logger;
            this.passwordHasher = passwordHasher;
        }

        public async Task<IDataResponse<User?>> GetAsync(Guid id)
        {
            return await usersAccessor.GetAsync(id);
        }

        public async Task<IDataResponse<User?>> PostAsync(PostUser user)
        {
            // Make sure the user doesn't exist.
            var response = await usersAccessor.GetByUserNameAsync(user.UserName);
            if (response.Result != ModelResult.NotFound)
            {
                logger.LogError($"Failed to post user {user.UserName}. A user of this user name already exists.");
                return new DataResponse<User?>(null, ModelResult.AlreadyExists);
            }

            // Don't allow any user to create new Admin or Utility user types.
            if (user.UserType == UserType.Admin || user.UserType == UserType.Utility)
            {
                logger.LogError($"Failed to post user {user.UserName}. A user of type Admin or Utility cannot be created.");
                return new DataResponse<User?>(null, ModelResult.RequestInvalid);
            }

            // Generate the hash data for the user.
            var hashData = passwordHasher.GenerateNewHash(user.Password);
            var hashedUser = new PostHashedUser
            {
                UserName = user.UserName,
                HashData = hashData,
                UserType = user.UserType,
                GameId = user.GameId
            };

            // Save the user with hash data to the database.
            return await usersAccessor.PostAsync(hashedUser);
        }

        public async Task<IDataResponse<User?>> PatchAsync(PatchUser user)
        {
            // Make sure the user exists.
            var response = await usersAccessor.GetAsync(user.Id);
            if (response.Result != ModelResult.OK)
            {
                logger.LogError($"Failed to patch user {user.Id}. A user of this ID does not exist.");
                return new DataResponse<User?>(null, ModelResult.NotFound);
            }

            // Don't allow any user to patch Admin or Utility user types.
            var storedUser = response.Data!;
            if (storedUser.UserType == UserType.Admin || storedUser.UserType == UserType.Utility)
            {
                logger.LogError($"Failed to patch user {storedUser.UserName}. A user of type Admin or Utility cannot be patched.");
                return new DataResponse<User?>(null, ModelResult.RequestInvalid);
            }

            // Return the patched user.
            return await usersAccessor.PatchAsync(user);
        }

        public async Task<IDataResponse<User?>> DeleteAsync(Guid id)
        {
            // Make sure the user exists.
            var response = await usersAccessor.GetAsync(id);
            if (response.Result != ModelResult.OK)
            {
                logger.LogError($"Failed to delete user {id}. A user of this ID does not exist.");
                return new DataResponse<User?>(null, ModelResult.NotFound);
            }

            // Don't allow any user to create new Admin or Utility user types.
            var user = response.Data!;
            if (user.UserType == UserType.Admin || user.UserType == UserType.Utility)
            {
                logger.LogError($"Failed to delete user {user.UserName}. A user of type Admin or Utility cannot be deleted.");
                return new DataResponse<User?>(null, ModelResult.RequestInvalid);
            }

            // Return the deleted user.
            return await usersAccessor.DeleteAsync(id);
        }
    }
}
