using SpyderByteAPI.DataAccess;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.DataAccess.Abstract.Accessors;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Helpers.Authentication;
using SpyderByteAPI.Models.Users;
using SpyderByteAPI.Services.Users.Abstract;

namespace SpyderByteAPI.Services.Users
{
    public class UserService : IUsersService
    {
        private readonly IUsersAccessor usersAccessor;
        private readonly ILogger<UserService> logger;
        private readonly PasswordHasher passwordHasher;

        public UserService(IUsersAccessor usersAccessor, ILogger<UserService> logger, PasswordHasher passwordHasher)
        {
            this.usersAccessor = usersAccessor;
            this.logger = logger;
            this.passwordHasher = passwordHasher;
        }

        public async Task<IDataResponse<User?>> PostAsync(PostUser user)
        {
            // Don't allow any user to create new Admin or Utility user types.
            if (user.UserType == UserType.Admin || user.UserType == UserType.Utility)
            {
                logger.LogError($"Failed to post user {user.Id}. A user of type Admin or Utility cannot be created.");
                return new DataResponse<User?>(null, ModelResult.RequestInvalid);
            }

            // Make sure the user doesn't exist.
            var response = await usersAccessor.GetAsync(user.Id);
            if (response.Result == ModelResult.OK)
            {
                logger.LogError($"Failed to post user {user.Id}. A user of this ID already exists.");
                return new DataResponse<User?>(null, ModelResult.AlreadyExists);
            }

            // Generate the hash data for the user.
            var hashData = passwordHasher.GenerateNewHash(user.Password);
            var hashedUser = new PostHashedUser
            {
                Id = user.Id,
                HashData = hashData,
                UserType = user.UserType
            };

            // Save the user with hash data to the database.
            return await usersAccessor.PostAsync(hashedUser);
        }

        public async Task<IDataResponse<User?>> DeleteAsync(string id)
        {
            // Make sure the user doesn't exist.
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
                logger.LogError($"Failed to delete user {user.Id}. A user of type Admin or Utility cannot be deleted.");
                return new DataResponse<User?>(null, ModelResult.RequestInvalid);
            }

            // Return the deleted user.
            return await usersAccessor.DeleteAsync(user.Id);
        }
    }
}
