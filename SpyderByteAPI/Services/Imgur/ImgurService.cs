using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpyderByteAPI.DataAccess;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Extensions;
using SpyderByteAPI.Models.Imgur;
using SpyderByteAPI.Services.Imgur.Abstract;
using System.Net.Http.Headers;

namespace SpyderByteAPI.Services.Imgur
{
    public class ImgurService : IImgurService
    {
        private readonly IConfiguration configuration;
        private IHttpClientFactory httpClientFactory;

        private string url;
        private string version;
        private string imageEndpoint = "image";
        private string authEndpoint = "oauth2/token";

        private string accessToken;
        private string refreshToken;

        public ImgurService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;

            url = configuration["Imgur:Url"] ?? string.Empty;
            version = configuration["Imgur:Version"] ?? string.Empty;

            accessToken = string.Empty;
            refreshToken = configuration["Imgur:RefreshToken"] ?? string.Empty;
        }

        public async Task<IDataResponse<PostImageResponse>> PostImageAsync(IFormFile file, string albumHash, string title)
        {
            // Get the access token. If neither are available, we can't proceed.
            var currentAccessToken = await getAccessToken();
            if (string.IsNullOrEmpty(currentAccessToken))
            {
                return new DataResponse<PostImageResponse>(new PostImageResponse(), ModelResult.Unauthorized);
            }

            // Make a request using the current access token.
            var response = await postImage(file, albumHash, title, currentAccessToken);

            // Make sure the POST was successful before processing the response.
            if (!response.IsSuccessStatusCode)
            {
                return new DataResponse<PostImageResponse>(new PostImageResponse(), ModelResult.Error);
            }

            // Get the data we need out of the Imgur response.
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseToken = JToken.Parse(responseJson);
            var imageUrl = responseToken?.SelectToken("data.link")?.Value<string>();
            var imageId = responseToken?.SelectToken("data.id")?.Value<string>();
            if (imageUrl == null || imageId == null)
            {
                return new DataResponse<PostImageResponse>(new PostImageResponse(), ModelResult.Error);
            }

            // Return the data.
            var postImageResponse = new PostImageResponse
            {
                Url = imageUrl,
                ImageId = imageId
            };
            return new DataResponse<PostImageResponse>(postImageResponse, ModelResult.OK);
        }

        public async Task<IDataResponse<bool>> DeleteImageAsync(string imageId)
        {
            // Get the access and refresh tokens. If neither are available, we can't proceed.
            var currentAccessToken = await getAccessToken();
            if (string.IsNullOrEmpty(currentAccessToken))
            {
                return new DataResponse<bool>(false, ModelResult.Unauthorized);
            }

            // Make a request using the current access token.
            var response = await deleteImage(imageId, currentAccessToken);

            // Make sure the DELETE was successful.
            if (response.IsSuccessStatusCode)
            {
                return new DataResponse<bool>(true, ModelResult.OK);
            }
            else
            {
                return new DataResponse<bool>(false, ModelResult.Error);
            }
        }

        private async Task<string> getAccessToken()
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                var newAccessToken = await generateAccessToken(refreshToken);
                if (!string.IsNullOrEmpty(newAccessToken))
                {
                    accessToken = newAccessToken;
                }
            }

            return accessToken;
        }

        private async Task<string> generateAccessToken(string refreshToken)
        {
            // We can't generate an access token without the refresh token.
            if (string.IsNullOrEmpty(refreshToken)) return string.Empty;

            using (var httpClient = httpClientFactory.CreateClient())
            {
                var requestContent = new MultipartFormDataContent();

                // Refresh Token
                var refreshTokenContent = new StringContent(refreshToken);
                requestContent.Add(refreshTokenContent, "refresh_token");

                // Client ID
                var clientIdContent = new StringContent(configuration["Imgur:ClientId"] ?? string.Empty);
                requestContent.Add(clientIdContent, "client_id");

                // Client Secret
                var clientSecretContent = new StringContent(configuration["Imgur:ClientSecret"] ?? string.Empty);
                requestContent.Add(clientSecretContent, "client_secret");

                // Grant Type
                var grantTypeContent = new StringContent("refresh_token");
                requestContent.Add(grantTypeContent, "grant_type");

                var response = await httpClient.PostAsync(url + authEndpoint, requestContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<IDictionary<string, string>>(responseJson);
                    if (responseObject != null)
                    {
                        return responseObject["access_token"];
                    }
                }
            }

            return string.Empty;
        }

        private async Task<HttpResponseMessage> postImage(IFormFile image, string albumHash, string title, string accessToken)
        {
            using (var httpClient = httpClientFactory.CreateClient())
            {
                var requestContent = new MultipartFormDataContent();

                // Authorize
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                // Image
                var imageContent = new ByteArrayContent(await image.GetBytes());
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(image.ContentType);
                requestContent.Add(imageContent, "image", image.FileName);

                // Album
                var albumContent = new StringContent(albumHash);
                requestContent.Add(albumContent, "album");

                // Title
                var titleContent = new StringContent(title);
                requestContent.Add(titleContent, "title");

                return await httpClient.PostAsync(url + version + "/" + imageEndpoint, requestContent);
            }
        }

        private async Task<HttpResponseMessage> deleteImage(string imageId, string accessToken)
        {
            using (var httpClient = httpClientFactory.CreateClient())
            {
                var requestContent = new MultipartFormDataContent();

                // Authorize
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                return await httpClient.DeleteAsync(url + version + "/" + imageEndpoint + "/" + imageId);
            }
        }
    }
}
