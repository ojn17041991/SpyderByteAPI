﻿using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using SpyderByteDataAccess.Accessors.Users.Abstract;
using SpyderByteDataAccess.Transactions.Factories.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Flags;
using SpyderByteResources.Helpers.Encoding;
using SpyderByteResources.Models.Responses;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Models.Users;
using SpyderByteServices.Services.Password.Abstract;
using SpyderByteServices.Services.Users.Abstract;

namespace SpyderByteServices.Services.Users
{
    public class UsersService(
        ITransactionFactory transactionFactory,
        IUsersAccessor usersAccessor,
        IMapper mapper,
        ILogger<UsersService> logger,
        IPasswordService passwordService,
        IFeatureManager featureManager) : IUsersService
    {
        private readonly ITransactionFactory transactionFactory = transactionFactory;
        private readonly IUsersAccessor usersAccessor = usersAccessor;
        private readonly IMapper mapper = mapper;
        private readonly ILogger<UsersService> logger = logger;
        private readonly IPasswordService passwordService = passwordService;
        private readonly IFeatureManager featureManager = featureManager;

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
                logger.LogError($"Failed to post user {LogEncoder.Encode(user.UserName)}. A user of this user name already exists.");
                return new DataResponse<User?>(null, ModelResult.AlreadyExists);
            }

            // Don't allow any user to create new Admin or Utility user types.
            if (await featureManager.IsEnabledAsync(FeatureFlags.AllowWriteOperationsOnNonRestrictedUsers) == false)
            {
                if (user.UserType == UserType.Admin || user.UserType == UserType.Utility)
                {
                    logger.LogError($"Failed to post user {LogEncoder.Encode(user.UserName)}. A user of type Admin or Utility cannot be created.");
                    return new DataResponse<User?>(null, ModelResult.RequestInvalid);
                }
            }

            // Generate the hash data for the user.
            var hashData = passwordService.GenerateNewHash(user.Password);
            var dataServiceUser = mapper.Map<SpyderByteDataAccess.Models.Users.PostUser>(user);
            dataServiceUser.HashData = mapper.Map<SpyderByteDataAccess.Models.Authentication.HashData>(hashData);

            // Save the user with hash data to the database.
            using (var transaction = await transactionFactory.CreateAsync())
            {
                var response = await usersAccessor.PostAsync(dataServiceUser);
                if (response.Result == ModelResult.Created)
                {
                    await transaction.CommitAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Users.User?>>(response);
                }
                else
                {
                    await transaction.RollbackAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Users.User?>>(response);
                }
            }
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
            if (await featureManager.IsEnabledAsync(FeatureFlags.AllowWriteOperationsOnNonRestrictedUsers) == false)
            {
                var storedUser = userResponse.Data!;
                if (storedUser.UserType == UserType.Admin || storedUser.UserType == UserType.Utility)
                {
                    logger.LogError($"Failed to patch user {LogEncoder.Encode(storedUser.UserName)}. A user of type Admin or Utility cannot be patched.");
                    return new DataResponse<User?>(null, ModelResult.RequestInvalid);
                }
            }

            // Return the patched user.
            using (var transaction = await transactionFactory.CreateAsync())
            {
                var response = await usersAccessor.PatchAsync(mapper.Map<SpyderByteDataAccess.Models.Users.PatchUser>(user));
                if (response.Result == ModelResult.OK)
                {
                    await transaction.CommitAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Users.User?>>(response);
                }
                else
                {
                    await transaction.RollbackAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Users.User?>>(response);
                }
            }
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
            if (await featureManager.IsEnabledAsync(FeatureFlags.AllowWriteOperationsOnNonRestrictedUsers) == false)
            {
                var user = userResponse.Data!;
                if (user.UserType == UserType.Admin || user.UserType == UserType.Utility)
                {
                    logger.LogError($"Failed to delete user {LogEncoder.Encode(user.UserName)}. A user of type Admin or Utility cannot be deleted.");
                    return new DataResponse<User?>(null, ModelResult.RequestInvalid);
                }
            }

            // Return the deleted user.
            using (var transaction = await transactionFactory.CreateAsync())
            {
                var response = await usersAccessor.DeleteAsync(id);
                if (response.Result == ModelResult.OK)
                {
                    await transaction.CommitAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Users.User?>>(response);
                }
                else
                {
                    await transaction.RollbackAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Users.User?>>(response);
                }
            }
        }
    }
}
