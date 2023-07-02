using Microsoft.EntityFrameworkCore;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.DataAccess.Abstract.Accessors;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Jams;

namespace SpyderByteAPI.DataAccess.Accessors
{
    public class JamsAccessor : IJamsAccessor
    {
        private ApplicationDbContext context;
        private ILogger<JamsAccessor> logger;

        public JamsAccessor(ApplicationDbContext context, ILogger<JamsAccessor> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<IDataResponse<IList<Jam>?>> GetAllAsync()
        {
            try
            {
                IList<Jam>? data = await context.Jams.ToListAsync();
                return new DataResponse<IList<Jam>?>(data, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to get all jams.", e);
                return new DataResponse<IList<Jam>?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<Jam?>> GetSingleAsync(int id)
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
                Jam? storedJam = await context.Jams.SingleOrDefaultAsync(j => j.Name.ToLower() == jam.Name.ToLower());
                if (storedJam != null)
                {
                    return new DataResponse<Jam?>(storedJam, ModelResult.AlreadyExists);
                }

                Jam mappedJam = new Jam
                {
                    Name = jam.Name,
                    ImgurUrl = jam.ImgurUrl,
                    ItchUrl = jam.ItchUrl
                };

                await context.Jams.AddAsync(mappedJam);
                await context.SaveChangesAsync();

                return new DataResponse<Jam?>(mappedJam, ModelResult.Created);
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

                if (patchedJam?.Name != null && patchedJam.Name == string.Empty)
                {
                    storedJam.Name = patchedJam.Name;
                }

                if (patchedJam?.ImgurUrl != null && patchedJam.ImgurUrl != string.Empty)
                {
                    storedJam.ImgurUrl = patchedJam.ImgurUrl;
                }

                if (patchedJam?.ItchUrl != null && patchedJam.ItchUrl != string.Empty)
                {
                    storedJam.ItchUrl = patchedJam.ItchUrl;
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

        public async Task<IDataResponse<Jam?>> DeleteAsync(int id)
        {
            try
            {
                Jam? jam = await context.Jams.SingleOrDefaultAsync(j => j.Id == id);
                if (jam == null)
                {
                    return new DataResponse<Jam?>(jam, ModelResult.NotFound);
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
    }
}
