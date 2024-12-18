﻿using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteDataAccess.Accessors.Games;
using SpyderByteDataAccess.Contexts;
using SpyderByteDataAccess.Models.Games;

namespace SpyderByteTest.DataAccess.GamesAccessorTests.Helpers
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

            Accessor = new GamesAccessor(_context, logger.Object);
        }

        public async Task<Game> AddGame()
        {
            var game = _fixture.Create<Game>();
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return game;
        }

        public async Task<IEnumerable<Game>> AddGames(int numGames)
        {
            IList<Game> games = new List<Game>();

            for (int i = 0; i < numGames; ++i)
            {
                var game = _fixture.Create<Game>();
                _context.Games.Add(game);
                games.Add(game);
            }

            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return games;
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
    }
}