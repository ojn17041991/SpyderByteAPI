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
                User? user = await context.Users
                    .Include(u => u.UserJam)
                        .ThenInclude(uj => uj!.Jam)
                    .SingleOrDefaultAsync(u => u.Id == id);
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

                if (user.JamId != null)
                {
                    var jam = await context.Jams.SingleOrDefaultAsync(j => j.Id == user.JamId);
                    if (jam == null)
                    {
                        logger.LogInformation($"Unable to post user {user.Id}. There is no jam for the ID {user.JamId}.");
                        return new DataResponse<User?>(null, ModelResult.NotFound);
                    }

                    var userJam = await context.UserJams.SingleOrDefaultAsync(uj => uj.JamId == jam.Id);
                    if (userJam != null)
                    {
                        logger.LogInformation($"Unable to post user {user.Id}. The jam ID {jam.Id} is already assigned to another user.");
                        return new DataResponse<User?>(null, ModelResult.AlreadyExists);
                    }

                    var mappedUserJam = new UserJam
                    {
                        JamId = jam.Id,
                        Jam = jam,
                        UserId = mappedUser.Id,
                        User = mappedUser
                    };

                    await context.UserJams.AddAsync(mappedUserJam);
                }

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
                User? user = await context.Users
                    .Include(u => u.UserJam)
                    .SingleOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    logger.LogInformation($"Unable to delete user. Could not find a user of ID {id}.");
                    return new DataResponse<User?>(user, ModelResult.NotFound);
                }

                // If the user has a user-jam relationship, we need to delete that too.
                if (user.UserJam != null)
                {
                    context.UserJams.Remove(user.UserJam);
                }

                context.Users.Remove(user);

                await context.SaveChangesAsync();

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
