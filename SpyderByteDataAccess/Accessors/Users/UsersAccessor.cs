﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpyderByteDataAccess.Accessors.Users.Abstract;
using SpyderByteDataAccess.Contexts;
using SpyderByteDataAccess.Models.Users;
using SpyderByteResources.Enums;
using SpyderByteResources.Helpers.Encoding;
using SpyderByteResources.Models.Responses;
using SpyderByteResources.Models.Responses.Abstract;

namespace SpyderByteDataAccess.Accessors.Users
{
    public class UsersAccessor(ApplicationDbContext context, ILogger<UsersAccessor> logger) : IUsersAccessor
    {
        private readonly ApplicationDbContext context = context;
        private readonly ILogger<UsersAccessor> logger = logger;

        public async Task<IDataResponse<User?>> GetAsync(Guid id)
        {
            try
            {
                User? user = await context.Users
                    .Include(u => u.UserGame)
                        .ThenInclude(uj => uj!.Game)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(u => u.Id == id);
                return new DataResponse<User?>(user, user == null ? ModelResult.NotFound : ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Failed to get user {id}.");
                return new DataResponse<User?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<User?>> GetByUserNameAsync(string userName)
        {
            try
            {
                User? user = await context.Users
                    .Include(u => u.UserGame)
                        .ThenInclude(uj => uj!.Game)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(u => u.UserName == userName);
                return new DataResponse<User?>(user, user == null ? ModelResult.NotFound : ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Failed to get user {LogEncoder.Encode(userName)}.");
                return new DataResponse<User?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<User?>> PostAsync(PostUser user)
        {
            try
            {
                bool userExists = await context.Users.AnyAsync(u => u.UserName == user.UserName);
                if (userExists)
                {
                    logger.LogInformation($"Unable to post user {user.UserName}. A user of this ID already exists.");
                    return new DataResponse<User?>(null, ModelResult.AlreadyExists);
                }

                User mappedUser = new()
                {
                    UserName = user.UserName,
                    Hash = user.HashData.Hash,
                    Salt = user.HashData.Salt,
                    UserType = user.UserType
                };

                if (user.GameId != null)
                {
                    var game = await context.Games.SingleOrDefaultAsync(g => g.Id == user.GameId);
                    if (game == null)
                    {
                        logger.LogInformation($"Unable to post user {LogEncoder.Encode(user.UserName)}. There is no game for the ID {user.GameId}.");
                        return new DataResponse<User?>(null, ModelResult.NotFound);
                    }

                    var userGame = await context.UserGames.SingleOrDefaultAsync(uj => uj.GameId == game.Id);
                    if (userGame != null)
                    {
                        logger.LogInformation($"Unable to post user {user.UserName}. The game ID {game.Id} is already assigned to another user.");
                        return new DataResponse<User?>(null, ModelResult.AlreadyExists);
                    }

                    var mappedUserGame = new UserGame
                    {
                        GameId = game.Id,
                        Game = game,
                        UserId = mappedUser.Id,
                        User = mappedUser
                    };

                    await context.UserGames.AddAsync(mappedUserGame);
                }

                await context.Users.AddAsync(mappedUser);

                await context.SaveChangesAsync();

                return new DataResponse<User?>(mappedUser, ModelResult.Created);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to post user.");
                return new DataResponse<User?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<User?>> PatchAsync(PatchUser user)
        {
            try
            {
                User? storedUser = await context.Users.SingleOrDefaultAsync(u => u.Id == user.Id);
                if (storedUser == null)
                {
                    logger.LogInformation($"Unable to patch user. Could not find a user of ID {user.Id}.");
                    return new DataResponse<User?>(null, ModelResult.NotFound);
                }

                if (user.GameId != null)
                {
                    var gameExists = await context.Games.AnyAsync(g => g.Id == user.GameId);
                    if (gameExists == false)
                    {
                        logger.LogInformation($"Unable to patch user. Could not find a game of ID {user.GameId}.");
                        return new DataResponse<User?>(null, ModelResult.NotFound);
                    }

                    var gameAllocatedToUser = await context.UserGames.AnyAsync(ug => ug.GameId == user.GameId && ug.UserId != user.Id);
                    if (gameAllocatedToUser == true)
                    {
                        logger.LogInformation($"Unable to patch user. Game {user.GameId} has already been allocated to a user.");
                        return new DataResponse<User?>(null, ModelResult.AlreadyExists);
                    }

                    var storedUserGame = await context.UserGames.SingleOrDefaultAsync(ug => ug.UserId == user.Id);
                    if (storedUserGame == null)
                    {
                        // Assign the game to the user as a new record.
                        var userGame = new UserGame
                        {
                            UserId = user.Id,
                            GameId = user.GameId.Value
                        };
                        await context.UserGames.AddAsync(userGame);
                    }
                    else
                    {
                        // Update the existing record.
                        storedUserGame.GameId = user.GameId.Value;
                    }
                }

                await context.SaveChangesAsync();

                return new DataResponse<User?>(storedUser, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to patch user.");
                return new DataResponse<User?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<User?>> DeleteAsync(Guid id)
        {
            try
            {
                User? user = await context.Users
                    .Include(u => u.UserGame)
                    .SingleOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    logger.LogInformation($"Unable to delete user. Could not find a user of ID {id}.");
                    return new DataResponse<User?>(null, ModelResult.NotFound);
                }

                // OJN: Can't this be configured in the DbContext to automatically delete these records?
                // If the user has a user-game relationship, we need to delete that too.
                if (user.UserGame != null)
                {
                    context.UserGames.Remove(user.UserGame);
                }

                context.Users.Remove(user);

                await context.SaveChangesAsync();

                return new DataResponse<User?>(user, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Failed to delete user {id}.");
                return new DataResponse<User?>(null, ModelResult.Error);
            }
        }
    }
}
