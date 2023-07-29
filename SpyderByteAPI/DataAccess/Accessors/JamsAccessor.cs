using Microsoft.EntityFrameworkCore;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.DataAccess.Abstract.Accessors;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Games;
using SpyderByteAPI.Models.Jams;
using SpyderByteAPI.Services.Imgur.Abstract;

namespace SpyderByteAPI.DataAccess.Accessors
{
    public class JamsAccessor : IJamsAccessor
    {
        private ApplicationDbContext context;
        private ILogger<JamsAccessor> logger;
        private IConfiguration configuration;
        private IImgurService imgurService;

        public JamsAccessor(ApplicationDbContext context, ILogger<JamsAccessor> logger, IConfiguration configuration, IImgurService imgurService)
        {
            this.context = context;
            this.logger = logger;
            this.configuration = configuration;
            this.imgurService = imgurService;
        }

        public async Task<IDataResponse<IList<Jam>?>> GetAllAsync()
        {
            try
            {
                IList<Jam>? data = await context.Jams.OrderBy(j => j.PublishDate).ToListAsync();
                return new DataResponse<IList<Jam>?>(data, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to get all jams.", e);
                return new DataResponse<IList<Jam>?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<Jam?>> GetSingleAsync(Guid id)
        {
            try
            {
                Jam? jam = await context.Jams.SingleOrDefaultAsync(j => j.Id == id);
                return new DataResponse<Jam?>(jam, jam == null ? ModelResult.NotFound : ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to get single jam.", e);
                return new DataResponse<Jam?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<Jam?>> PostAsync(PostJam jam)
        {
            try
            {
                if (jam.Image == null)
                {
                    return new DataResponse<Jam?>(null, ModelResult.RequestDataIncomplete);
                }

                Jam? storedJam = await context.Jams.SingleOrDefaultAsync(j => j.Name.ToLower() == jam.Name.ToLower());
                if (storedJam != null)
                {
                    return new DataResponse<Jam?>(storedJam, ModelResult.AlreadyExists);
                }

                var response = await imgurService.PostImageAsync(jam.Image, configuration["Imgur:JamsAlbumHash"] ?? string.Empty, Path.GetFileNameWithoutExtension(jam.Image.FileName));
                if (response.Result != ModelResult.OK)
                {
                    return new DataResponse<Jam?>(null, response.Result);
                }

                Jam mappedJam = new Jam
                {
                    Name = jam.Name,
                    ImgurUrl = response.Data.Url,
                    ImgurImageId = response.Data.ImageId,
                    ItchUrl = jam.ItchUrl,
                    PublishDate = jam.PublishDate
                };

                await context.Jams.AddAsync(mappedJam);
                await context.SaveChangesAsync();

                return new DataResponse<Jam?>(mappedJam, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to post jam.", e);
                return new DataResponse<Jam?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<Jam?>> PatchAsync(PatchJam patchedJam)
        {
            try
            {
                Jam? storedJam = await context.Jams.SingleOrDefaultAsync(j => j.Id == patchedJam.Id);
                if (storedJam == null)
                {
                    return new DataResponse<Jam?>(storedJam, ModelResult.NotFound);
                }

                // First, check if the image is being updated.
                if (patchedJam?.Image != null)
                {
                    // Delete the old image from Imgur.
                    var imgurDeleteSuccessful = await imgurService.DeleteImageAsync(storedJam.ImgurImageId);
                    if (!imgurDeleteSuccessful.Data)
                    {
                        logger.LogWarning($"Failed to delete image from Imgur account for jam {storedJam.Name}. Continuing to database update.");
                    }

                    // Post the new image.
                    var response = await imgurService.PostImageAsync(patchedJam.Image, configuration["Imgur:JamsAlbumHash"] ?? string.Empty, Path.GetFileNameWithoutExtension(patchedJam.Image.FileName));
                    if (response.Result != ModelResult.OK)
                    {
                        return new DataResponse<Jam?>(null, response.Result);
                    }

                    storedJam.ImgurUrl = response.Data.Url;
                    storedJam.ImgurImageId = response.Data.ImageId;
                }

                if (patchedJam?.Name != null && patchedJam.Name != string.Empty)
                {
                    storedJam.Name = patchedJam.Name;
                }

                if (patchedJam?.ItchUrl != null && patchedJam.ItchUrl != string.Empty)
                {
                    storedJam.ItchUrl = patchedJam.ItchUrl;
                }

                if (patchedJam?.PublishDate != null)
                {
                    storedJam.PublishDate = (DateTime)patchedJam.PublishDate;
                }

                await context.SaveChangesAsync();

                return new DataResponse<Jam?>(storedJam, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to patch jam.", e);
                return new DataResponse<Jam?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<Jam?>> DeleteAsync(Guid id)
        {
            try
            {
                Jam? jam = await context.Jams.SingleOrDefaultAsync(j => j.Id == id);
                if (jam == null)
                {
                    return new DataResponse<Jam?>(jam, ModelResult.NotFound);
                }

                var imgurDeleteSuccessful = await imgurService.DeleteImageAsync(jam.ImgurImageId);
                if (!imgurDeleteSuccessful.Data)
                {
                    logger.LogWarning($"Failed to delete image from Imgur account for jam {jam.Name}. Continuing to database deletion.");
                }

                context.Jams.Remove(jam);
                await context.SaveChangesAsync();

                return new DataResponse<Jam?>(jam, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to delete jam.", e);
                return new DataResponse<Jam?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<IList<Jam>?>> DeleteAllAsync()
        {
            try
            {
                var jams = await context.Jams.ToListAsync();

                foreach (var jam in jams)
                {
                    var imgurDeleteSuccessful = await imgurService.DeleteImageAsync(jam.ImgurImageId);
                    if (!imgurDeleteSuccessful.Data)
                    {
                        logger.LogWarning($"Failed to delete image from Imgur account for jam {jam.Name}. Continuing to database deletion.");
                    }
                }

                context.Jams.RemoveRange(jams);
                await context.SaveChangesAsync();

                return new DataResponse<IList<Jam>?>(jams, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to clear jams.", e);
                return new DataResponse<IList<Jam>?>(null, ModelResult.Error);
            }
        }
    }
}
