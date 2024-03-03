﻿using Microsoft.EntityFrameworkCore;
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
                    .Include(u => u.UserGame)
                        .ThenInclude(uj => uj!.Game)
                    .SingleOrDefaultAsync(u => u.UserName == id);
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
                bool userExists = await context.Users.AnyAsync(u => u.UserName == user.UserName);
                if (userExists)
                {
                    logger.LogInformation($"Unable to post user {user.UserName}. A user of this ID already exists.");
                    return new DataResponse<User?>(null, ModelResult.AlreadyExists);
                }

                User mappedUser = new User
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
                        logger.LogInformation($"Unable to post user {user.UserName}. There is no game for the ID {user.GameId}.");
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
                logger.LogError("Failed to post user.", e);
                return new DataResponse<User?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<User?>> DeleteAsync(string id)
        {
            try
            {
                User? user = await context.Users
                    .Include(u => u.UserGame)
                    .SingleOrDefaultAsync(u => u.UserName == id);

                if (user == null)
                {
                    logger.LogInformation($"Unable to delete user. Could not find a user of ID {id}.");
                    return new DataResponse<User?>(user, ModelResult.NotFound);
                }

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
                logger.LogError($"Failed to delete user {id}.", e);
                return new DataResponse<User?>(null, ModelResult.Error);
            }
        }
    }
}