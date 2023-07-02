using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.DataAccess.Abstract.Accessors;
using SpyderByteAPI.Enums;
using System.Net;

namespace SpyderByteAPI.DataAccess.Accessors
{
    public class LeaderboardAccessor : ILeaderboardAccessor
    {
        private IHttpClientFactory httpClientFactory;
        private ILogger<GamesAccessor> logger;
        private const string serviceName = "dreamlo";
        private const string baseAddress = "http://dreamlo.com/lb";


        public LeaderboardAccessor(IHttpClientFactory httpClientFactory, ILogger<GamesAccessor> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task<IDataResponse<string>> GetAsync(string key)
        {
            try
            {
                using (HttpClient httpClient = httpClientFactory.CreateClient(serviceName))
                {
                    HttpResponseMessage response = await httpClient.GetAsync($"{baseAddress}/{key}/json/");

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string data = "var jsonp = " + await response.Content.ReadAsStringAsync();
                        return new DataResponse<string>(data, ModelResult.OK);
                    }
                    else
                    {
                        logger.LogError($"Failed to get dreamlo leaderboard. {response.StatusCode} returned.");
                        return new DataResponse<string>(string.Empty, ModelResult.Error);
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError("Failed to get dreamlo leaderboard.", e);
                return new DataResponse<string>(string.Empty, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<string>> PostAsync(string key, string name, int score)
        {
            try
            {
                using (HttpClient httpClient = httpClientFactory.CreateClient(serviceName))
                {
                    HttpResponseMessage response = await httpClient.PostAsync($"{baseAddress}/{key}/add/{name}/{score}", null);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        return new DataResponse<string>(data, ModelResult.OK);
                    }
                    else
                    {
                        logger.LogError($"Failed to post to dreamlo leaderboard. {response.StatusCode} returned.");
                        return new DataResponse<string>(string.Empty, ModelResult.Error);
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError("Failed to post to dreamlo leaderboard.", e);
                return new DataResponse<string>(string.Empty, ModelResult.Error);
            }
        }
    }
}
