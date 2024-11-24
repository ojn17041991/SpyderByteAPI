﻿using SpyderByteResources.Responses.Abstract;
using SpyderByteServices.Models.Games;

namespace SpyderByteServices.Services.Games.Abstract
{
    public interface IGamesService
    {
        Task<IDataResponse<IList<Game>?>> GetAllAsync(string? filter, int page, int count, string order, string direction);

        Task<IDataResponse<Game?>> GetSingleAsync(Guid id);

        Task<IDataResponse<Game?>> PostAsync(PostGame game);

        Task<IDataResponse<Game?>> PatchAsync(PatchGame patchedGame);

        Task<IDataResponse<Game?>> DeleteAsync(Guid id);
    }
}
