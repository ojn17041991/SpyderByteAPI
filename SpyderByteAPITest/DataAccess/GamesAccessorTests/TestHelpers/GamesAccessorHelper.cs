﻿using AutoFixture;
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
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

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
            _context.UserGames.Add(game.UserGame!);
            _context.LeaderboardGames.Add(game.LeaderboardGame!);
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

        public async Task RemoveUserGameRelationship(Guid id)
        {
            var game = await _context.Games
                .Include(g => g.UserGame)
                .SingleAsync(g => g.Id == id);

            if (game.UserGame != null)
            {
                _context.UserGames.Remove(game.UserGame);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveLeaderboardGameRelationship(Guid id)
        {
            var game = await _context.Games
                .Include(g => g.LeaderboardGame)
                .SingleAsync(g => g.Id == id);

            if (game.LeaderboardGame != null)
            {
                _context.LeaderboardGames.Remove(game.LeaderboardGame);
            }

            await _context.SaveChangesAsync();
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
                PublishDate = game.PublishDate,
                LeaderboardGame = game.LeaderboardGame,
                UserGame = game.UserGame
            };
        }
    }
}