using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteAPI.DataAccess;
using SpyderByteAPI.DataAccess.Accessors;
using SpyderByteAPI.Models.Games;
using SpyderByteAPI.Models.Leaderboard;

namespace SpyderByteAPITest.DataAccess.LeaderboardAccessorTests.Helper
{
    public class LeaderboardAccessorHelper
    {
        public LeaderboardAccessor Accessor;

        private readonly ApplicationDbContext _context;
        private readonly Fixture _fixture;

        public LeaderboardAccessorHelper()
        {
            _fixture = new Fixture();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var logger = new Mock<ILogger<LeaderboardAccessor>>();

            Accessor = new LeaderboardAccessor(_context, logger.Object);
        }

        public async Task<LeaderboardRecord> AddLeaderboardRecord()
        {
            var leaderboardRecord = _fixture.Create<LeaderboardRecord>();
            _context.LeaderboardRecords.Add(leaderboardRecord);
            await _context.SaveChangesAsync();
            return DeepClone(leaderboardRecord);
        }

        public async Task<List<LeaderboardRecord>> GetLeaderboardRecords()
        {
            return await _context.LeaderboardRecords.ToListAsync();
        }

        public PostLeaderboardRecord GeneratePostLeaderboardRecord()
        {
            return _fixture.Create<PostLeaderboardRecord>();
        }

        public async Task<LeaderboardRecord?> GetLeaderboardRecord(Guid id)
        {
            return await _context.LeaderboardRecords.SingleOrDefaultAsync(lr => lr.Id == id);
        }

        public async Task<Game> AddGame()
        {
            var game = _fixture.Create<Game>();
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            return game;
        }

        //public async Task<Jam> AddJam()
        //{
        //    var jam = _fixture.Create<Jam>();
        //    _context.Jams.Add(jam);
        //    await _context.SaveChangesAsync();
        //    return jam;
        //}

        private LeaderboardRecord DeepClone(LeaderboardRecord leaderboardRecord)
        {
            return new LeaderboardRecord
            {
                Id = leaderboardRecord.Id,
                Leaderboard = leaderboardRecord.Leaderboard,
                Player = leaderboardRecord.Player,
                Score = leaderboardRecord.Score,
                Timestamp = leaderboardRecord.Timestamp
            };
        }
    }
}
