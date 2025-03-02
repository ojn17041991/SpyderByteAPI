﻿using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteDataAccess.Accessors.Leaderboards;
using SpyderByteDataAccess.Contexts;
using SpyderByteDataAccess.Models.Games;
using SpyderByteDataAccess.Models.Leaderboards;

namespace SpyderByteTest.DataAccess.LeaderboardsAccessorTests.Helpers
{
    public class LeaderboardsAccessorHelper
    {
        public LeaderboardsAccessor Accessor;

        private readonly ApplicationDbContext _context;
        private readonly Fixture _fixture;

        public LeaderboardsAccessorHelper()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var logger = new Mock<ILogger<LeaderboardsAccessor>>();

            Accessor = new LeaderboardsAccessor(_context, logger.Object);
        }

        public async Task<Game> AddGameWithLeaderboard()
        {
            var game = _fixture.Create<Game>();
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return game;
        }

        public async Task<Game> AddGameWithoutLeaderboard()
        {
            var game = _fixture.Create<Game>();
            game.LeaderboardGame = null;
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return game;
        }

        public async Task<Leaderboard> AddLeaderboardWithRecords(int numRecords)
        {
            var leaderboard = _fixture.Create<Leaderboard>();
            leaderboard.LeaderboardRecords.Clear();
            _context.Leaderboards.Add(leaderboard);
            await _context.SaveChangesAsync();

            for (int i = 0; i < numRecords; ++i)
            {
                var leaderboardRecord = _fixture.Create<LeaderboardRecord>();
                leaderboardRecord.Leaderboard = leaderboard;
                leaderboardRecord.LeaderboardId = leaderboard.Id;
                _context.LeaderboardRecords.Add(leaderboardRecord);
            }

            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return leaderboard;
        }
        
        public async Task<Leaderboard> AddLeaderboardWithoutRecords()
        {
            var leaderboard = _fixture.Create<Leaderboard>();
            leaderboard.LeaderboardRecords = new List<LeaderboardRecord>();
            _context.Leaderboards.Add(leaderboard);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return leaderboard;
        }

        public async Task<Leaderboard> AddLeaderboardWithGame()
        {
            var leaderboard = _fixture.Create<Leaderboard>();
            _context.Leaderboards.Add(leaderboard);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return leaderboard;
        }

        public async Task<Leaderboard> AddLeaderboardWithoutGame()
        {
            var leaderboard = _fixture.Create<Leaderboard>();
            leaderboard.LeaderboardGame = null!;
            _context.Leaderboards.Add(leaderboard);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return leaderboard;
        }

        public async Task<List<Leaderboard>> GetLeaderboards()
        {
            return await _context.Leaderboards.ToListAsync();
        }

        public async Task<List<LeaderboardRecord>> GetLeaderboardRecords()
        {
            return await _context.LeaderboardRecords.ToListAsync();
        }

        public PostLeaderboard GeneratePostLeaderboard()
        {
            return _fixture.Create<PostLeaderboard>();
        }

        public PatchLeaderboard GeneratePatchLeaderboard()
        {
            return _fixture.Create<PatchLeaderboard>();
        }

        public PostLeaderboardRecord GeneratePostLeaderboardRecord()
        {
            return _fixture.Create<PostLeaderboardRecord>();
        }

        public async Task<Leaderboard?> GetLeaderboard(Guid id)
        {
            return await _context.Leaderboards.SingleOrDefaultAsync(lr => lr.Id == id);
        }

        public async Task<LeaderboardRecord?> GetLeaderboardRecord(Guid id)
        {
            return await _context.LeaderboardRecords.SingleOrDefaultAsync(lr => lr.Id == id);
        }
    }
}
