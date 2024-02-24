using Microsoft.EntityFrameworkCore;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.DataAccess.Abstract.Accessors;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Users;

namespace SpyderByteAPI.DataAccess.Accessors
{
    public class UsersAccessor : IUsersAccessor
    {
        private ApplicationDbContext context;
        private ILogger<UsersAccessor> logger;

        public UsersAccessor(ApplicationDbContext context, ILogger<UsersAccessor> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<IDataResponse<User?>> GetAsync(string id)
        {
            try
            {
                User? user = await context.Users.SingleOrDefaultAsync(g => g.Id == id);
                return new DataResponse<User?>(user, user == null ? ModelResult.NotFound : ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to get user {id}.", e);
                return new DataResponse<User?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<User?>> PostAsync(PostHashedUser user)
        {
            try
            {
                bool userExists = await context.Users.AnyAsync(u => u.Id == user.Id);
                if (userExists)
                {
                    logger.LogInformation($"Unable to post user {user.Id}. A user of this ID already exists.");
                    return new DataResponse<User?>(null, ModelResult.AlreadyExists);
                }

                User mappedUser = new User
                {
                    Id = user.Id,
                    Hash = user.HashData.Hash,
                    Salt = user.HashData.Salt,
                    UserType = user.UserType
                };

                await context.Users.AddAsync(mappedUser);
                await context.SaveChangesAsync();

                return new DataResponse<User?>(mappedUser, ModelResult.Created);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to post user.", e);
                return new DataResponse<User?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<User?>> DeleteAsync(string id)
        {
            try
            {
                User? user = await context.Users.SingleOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    logger.LogInformation($"Unable to delete user. Could not find a user of ID {id}.");
                    return new DataResponse<User?>(user, ModelResult.NotFound);
                }

                context.Users.Remove(user);
                await context.SaveChangesAsync();

                // Remove the hash and salt before returning the entity.
                user.Hash = string.Empty;
                user.Salt = string.Empty;

                return new DataResponse<User?>(user, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to delete user {id}.", e);
                return new DataResponse<User?>(null, ModelResult.Error);
            }
        }
    }
}
