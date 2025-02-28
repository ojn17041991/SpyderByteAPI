using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteDataAccess.Accessors.Users;
using SpyderByteDataAccess.Contexts;
using SpyderByteDataAccess.Models.Games;
using SpyderByteDataAccess.Models.Users;

namespace SpyderByteTest.DataAccess.UsersAccessorTests.Helpers
{
    public class UsersAccessorHelper
    {
        public UsersAccessor Accessor;

        private readonly ApplicationDbContext _context;
        private readonly Fixture _fixture;

        public UsersAccessorHelper()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var logger = new Mock<ILogger<UsersAccessor>>();

            Accessor = new UsersAccessor(_context, logger.Object);
        }

        public async Task<IList<User>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUser(Guid id)
        {
            return await _context.Users.SingleAsync(u => u.Id == id);
        }

        public async Task<User> AddUser()
        {
            var user = _fixture.Create<User>();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return user;
        }

        public async Task<User> AddUserWithoutGame()
        {
            var user = _fixture.Create<User>();
            user.UserGame = null!;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return user;
        }

        public async Task<Game> AddGame()
        {
            var game = _fixture.Create<Game>();
            game.UserGame = null!;
            game.LeaderboardGame = null!;
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return game;
        }

        public PostUser GeneratePostUser()
        {
            return _fixture.Create<PostUser>();
        }

        public PatchUser GeneratePatchUser()
        {
            return _fixture.Create<PatchUser>();
        }
    }
}
