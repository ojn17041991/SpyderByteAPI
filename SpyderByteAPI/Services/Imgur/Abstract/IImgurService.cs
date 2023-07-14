using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Models.Imgur;

namespace SpyderByteAPI.Services.Imgur.Abstract
{
    public interface IImgurService
    {
        Task<IDataResponse<PostImageResponse>> PostImageAsync(IFormFile file, string albumHash, string title);

        Task<IDataResponse<bool>> DeleteImageAsync(string imageId);
    }
}
