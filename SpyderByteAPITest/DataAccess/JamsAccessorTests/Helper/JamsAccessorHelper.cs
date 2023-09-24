using AutoFixture;
using SpyderByteAPI.DataAccess.Accessors;
using SpyderByteAPI.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteAPI.Services.Imgur.Abstract;
using SpyderByteAPI.Models.Imgur;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Jams;

namespace SpyderByteAPITest.DataAccess.JamsAccessorTests.Helper
{
    public class JamsAccessorHelper
    {
        public JamsAccessor Accessor;

        private readonly ApplicationDbContext _context;
        private readonly Fixture _fixture;

        public JamsAccessorHelper()
        {
            _fixture = new Fixture();
            _fixture.Customize<IFormFile>(f => f.FromFactory(() => new Mock<IFormFile>().Object));

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var logger = new Mock<ILogger<JamsAccessor>>();

            var configurationContents = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationContents)
                .Build();

            var imgurService = new Mock<IImgurService>();
            imgurService.Setup(i => i.PostImageAsync(
                It.IsAny<IFormFile>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            )).ReturnsAsync((IFormFile file, string albumHash, string title) =>
                new DataResponse<PostImageResponse>(_fixture.Create<PostImageResponse>(), ModelResult.OK)
            );
            imgurService.Setup(i => i.DeleteImageAsync(
                It.IsAny<string>()
            )).ReturnsAsync((string imageHash) =>
                new DataResponse<bool>(true, ModelResult.OK)
            );

            Accessor = new JamsAccessor(_context, logger.Object, configuration, imgurService.Object);
        }

        public async Task<Jam> AddJam()
        {
            var jam = _fixture.Create<Jam>();
            _context.Jams.Add(jam);
            await _context.SaveChangesAsync();
            return DeepClone(jam);
        }

        public async Task<List<Jam>> GetJams()
        {
            return await _context.Jams.ToListAsync();
        }

        public async Task<Jam?> GetJam(Guid id)
        {
            return await _context.Jams.SingleOrDefaultAsync(j => j.Id == id);
        }

        public PostJam GeneratePostJam()
        {
            return _fixture.Create<PostJam>();
        }

        public PatchJam GeneratePatchJam()
        {
            return _fixture.Create<PatchJam>();
        }

        private Jam DeepClone(Jam jam)
        {
            return new Jam
            {
                Id = jam.Id,
                Name = jam.Name,
                ItchUrl = jam.ItchUrl,
                ImgurUrl = jam.ImgurUrl,
                ImgurImageId = jam.ImgurImageId,
                PublishDate = jam.PublishDate
            };
        }
    }
}
