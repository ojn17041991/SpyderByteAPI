using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteDataAccess.Accessors.Games;
using SpyderByteDataAccess.Contexts;
using SpyderByteDataAccess.Models.Games;

namespace SpyderByteAPITest.DataAccess.GamesAccessorTests.Helper
{
    public class GamesAccessorHelper
    {
        public GamesAccessor Accessor;

        private readonly ApplicationDbContext _context;
        private readonly Fixture _fixture;

        public GamesAccessorHelper()
        {
            _fixture = new Fixture();
            _fixture.Customize<IFormFile>(f => f.FromFactory(() => new Mock<IFormFile>().Object));

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var logger = new Mock<ILogger<GamesAccessor>>();

            var configurationContents = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationContents)
                .Build();

            //var imgurService = new Mock<IImgurService>();
            //imgurService.Setup(i => i.PostImageAsync(
            //    It.IsAny<IFormFile>(),
            //    It.IsAny<string>(),
            //    It.IsAny<string>()
            //)).ReturnsAsync((IFormFile file, string albumHash, string title) =>
            //    new DataResponse<PostImageResponse>(_fixture.Create<PostImageResponse>(), ModelResult.OK)
            //);
            //imgurService.Setup(i => i.DeleteImageAsync(
            //    It.IsAny<string>()
            //)).ReturnsAsync((string imageHash) =>
            //    new DataResponse<bool>(true, ModelResult.OK)
            //);

            Accessor = new GamesAccessor(_context, logger.Object, configuration);
        }

        public async Task<Game> AddGame()
        {
            var game = _fixture.Create<Game>();
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            return DeepClone(game);
        }

        public async Task<IList<Game>> GetGames()
        {
            return await _context.Games.ToListAsync();
        }

        public async Task<Game?> GetGame(Guid id)
        {
            return await _context.Games.SingleOrDefaultAsync(g => g.Id == id);
        }

        public PostGame GeneratePostGame()
        {
            return _fixture.Create<PostGame>();
        }

        public PatchGame GeneratePatchGame()
        {
            return _fixture.Create<PatchGame>();
        }

        private Game DeepClone(Game game)
        {
            return new Game
            {
                Id = game.Id,
                Name = game.Name,
                HtmlUrl = game.HtmlUrl,
                ImgurUrl = game.ImgurUrl,
                ImgurImageId = game.ImgurImageId,
                PublishDate = game.PublishDate
            };
        }
    }
}