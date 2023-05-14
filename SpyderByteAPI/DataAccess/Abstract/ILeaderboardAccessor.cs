namespace SpyderByteAPI.DataAccess.Abstract
{
    public interface ILeaderboardAccessor
    {
        Task<IDataResponse<string>> GetAsync(string key);

        Task<IDataResponse<string>> PostAsync(string key, string name, int score);
    }
}
