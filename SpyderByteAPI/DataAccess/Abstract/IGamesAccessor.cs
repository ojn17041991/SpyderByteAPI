﻿using SpyderByteAPI.Models.Games;

namespace SpyderByteAPI.DataAccess.Abstract
{
    public interface IGamesAccessor
    {
        Task<IDataResponse<IList<Game>?>> GetAllAsync();

        Task<IDataResponse<Game?>> GetSingleAsync(int id);

        Task<IDataResponse<Game?>> PostAsync(PostGame game);

        Task<IDataResponse<Game?>> PatchAsync(PatchGame patchedGame);

        Task<IDataResponse<Game?>> DeleteAsync(int id);
    }
}
