using AutoMapper;
using Microsoft.Extensions.Logging;
using SpyderByteDataAccess.Accessors.Users.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses;
using SpyderByteResources.Responses.Abstract;
using SpyderByteServices.Helpers.Authentication;
using SpyderByteServices.Models.Users;
using SpyderByteServices.Services.Users.Abstract;

namespace SpyderByteServices.Services.Users
{
    public class UsersService : IUsersService
    {
        private readonly IUsersAccessor usersAccessor;
        private readonly IMapper mapper;
        private readonly ILogger<UsersService> logger;
        private readonly PasswordHasher passwordHasher;

        public UsersService(IUsersAccessor usersAccessor, IMapper mapper, ILogger<UsersService> logger, PasswordHasher passwordHasher)
        {
            this.usersAccessor = usersAccessor;
            this.mapper = mapper;
            this.logger = logger;
            this.passwordHasher = passwordHasher;
        }

        public async Task<IDataResponse<User?>> GetAsync(Guid id)
        {
            var response = await usersAccessor.GetAsync(id);
            return mapper.Map<DataResponse<SpyderByteServices.Models.Users.User?>>(response);
        }

        public async Task<IDataResponse<User?>> PostAsync(PostUser user)
        {
            // Make sure the user doesn't exist.
            var userResponse = await usersAccessor.GetByUserNameAsync(user.UserName);
            if (userResponse.Result != ModelResult.NotFound)
            {
                logger.LogError($"Failed to post user {user.UserName}. A user of this user name already exists.");
                return new DataResponse<User?>(null, ModelResult.AlreadyExists);
            }

            // Don't allow any user to create new Admin or Utility user types.
            //if (user.UserType == UserType.Admin || user.UserType == UserType.Utility)
            //{
            //    logger.LogError($"Failed to post user {user.UserName}. A user of type Admin or Utility cannot be created.");
            //    return new DataResponse<User?>(null, ModelResult.RequestInvalid);
            //}

            // Generate the hash data for the user.
            var hashData = passwordHasher.GenerateNewHash(user.Password);
            user.HashData = hashData;

            // Save the user with hash data to the database.
            var response = await usersAccessor.PostAsync(mapper.Map<SpyderByteDataAccess.Models.Users.PostUser>(user));
            return mapper.Map<DataResponse<SpyderByteServices.Models.Users.User?>>(response);
        }

        public async Task<IDataResponse<User?>> PatchAsync(PatchUser user)
        {
            // Make sure the user exists.
            var userResponse = await usersAccessor.GetAsync(user.Id);
            if (userResponse.Result != ModelResult.OK)
            {
                logger.LogError($"Failed to patch user {user.Id}. A user of this ID does not exist.");
                return new DataResponse<User?>(null, ModelResult.NotFound);
            }

            // Don't allow any user to patch Admin or Utility user types.
            var storedUser = userResponse.Data!;
            if (storedUser.UserType == UserType.Admin || storedUser.UserType == UserType.Utility)
            {
                logger.LogError($"Failed to patch user {storedUser.UserName}. A user of type Admin or Utility cannot be patched.");
                return new DataResponse<User?>(null, ModelResult.RequestInvalid);
            }

            // Return the patched user.
            var response = await usersAccessor.PatchAsync(mapper.Map<SpyderByteDataAccess.Models.Users.PatchUser>(user));
            return mapper.Map<DataResponse<SpyderByteServices.Models.Users.User?>>(response);
        }

        public async Task<IDataResponse<User?>> DeleteAsync(Guid id)
        {
            // Make sure the user exists.
            var userResponse = await usersAccessor.GetAsync(id);
            if (userResponse.Result != ModelResult.OK)
            {
                logger.LogError($"Failed to delete user {id}. A user of this ID does not exist.");
                return new DataResponse<User?>(null, ModelResult.NotFound);
            }

            // Don't allow any user to create new Admin or Utility user types.
            //var user = userResponse.Data!;
            //if (user.UserType == UserType.Admin || user.UserType == UserType.Utility)
            //{
            //    logger.LogError($"Failed to delete user {user.UserName}. A user of type Admin or Utility cannot be deleted.");
            //    return new DataResponse<User?>(null, ModelResult.RequestInvalid);
            //}

            // Return the deleted user.
            var response = await usersAccessor.DeleteAsync(id);
            return mapper.Map<DataResponse<SpyderByteServices.Models.Users.User?>>(response);
        }
    }
}
