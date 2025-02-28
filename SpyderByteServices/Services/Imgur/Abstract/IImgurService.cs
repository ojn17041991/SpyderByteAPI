using Microsoft.AspNetCore.Http;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Models.Imgur;

namespace SpyderByteServices.Services.Imgur.Abstract
{
    public interface IImgurService
    {
        Task<IDataResponse<PostImageResponse>> PostImageAsync(IFormFile file, string albumHash, string title);

        Task<IDataResponse<bool>> DeleteImageAsync(string imageId);
    }
}
