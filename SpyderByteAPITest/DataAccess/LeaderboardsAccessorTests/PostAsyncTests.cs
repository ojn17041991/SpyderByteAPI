using FluentAssertions.Execution;
using FluentAssertions;
using SpyderByteTest.DataAccess.LeaderboardsAccessorTests.Helpers;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses.Abstract;
using SpyderByteDataAccess.Models.Leaderboards;

namespace SpyderByteTest.DataAccess.LeaderboardsAccessorTests
{
    public class PostAsyncTests
    {
        private readonly LeaderboardsAccessorHelper _helper;
        private readonly LeaderboardsAccessorExceptionHelper _exceptionHelper;

        public PostAsyncTests()
        {
            _helper = new LeaderboardsAccessorHelper();
            _exceptionHelper = new LeaderboardsAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Add_Leaderboard_To_Accessor()
        {
            // Arrange
            var storedGame = await _helper.AddGameWithoutLeaderboard();
            var postLeaderboard = _helper.GeneratePostLeaderboard();
            postLeaderboard.GameId = storedGame.Id;

            // Act
            var returnedLeaderboard = await _helper.Accessor.PostAsync(postLeaderboard);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedLeaderboard.Should().NotBeNull();
                returnedLeaderboard.Result.Should().Be(ModelResult.Created);
                returnedLeaderboard.Data.Should().NotBeNull();
                returnedLeaderboard.Data!.LeaderboardGame.GameId.Should().Be(postLeaderboard.GameId);

                // Check the database.
                var storedLeaderboard = await _helper.GetLeaderboard(returnedLeaderboard.Data!.Id);
                storedLeaderboard.Should().NotBeNull();
                storedLeaderboard!.Id.Should().Be(returnedLeaderboard.Data!.Id);
                storedLeaderboard!.LeaderboardRecords.Should().BeEquivalentTo(returnedLeaderboard.Data!.LeaderboardRecords);
                storedLeaderboard!.LeaderboardGame.GameId.Should().Be(postLeaderboard.GameId);
            }
        }

        [Fact]
        public async Task Can_Not_Add_Leaderboard_To_Accessor_When_Game_Does_Not_Exist()
        {
            // Arrange
            var postLeaderboard = _helper.GeneratePostLeaderboard();

            // Act
            var returnedLeaderboard = await _helper.Accessor.PostAsync(postLeaderboard);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedLeaderboard.Should().NotBeNull();
                returnedLeaderboard.Result.Should().Be(ModelResult.NotFound);
                returnedLeaderboard.Data.Should().BeNull();
            }
        }

        [Fact]
        public async Task Can_Not_Add_Leaderboard_To_Accessor_When_Game_Already_Has_Leaderboard_Assigned()
        {
            // Arrange
            var storedGame = await _helper.AddGameWithLeaderboard();
            var postLeaderboard = _helper.GeneratePostLeaderboard();
            postLeaderboard.GameId = storedGame.Id;

            // Act
            var returnedLeaderboard = await _helper.Accessor.PostAsync(postLeaderboard);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedLeaderboard.Should().NotBeNull();
                returnedLeaderboard.Result.Should().Be(ModelResult.AlreadyExists);
                returnedLeaderboard.Data.Should().BeNull();
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange
            var postLeaderboard = _helper.GeneratePostLeaderboard();

            // Act
            Func<Task<IDataResponse<Leaderboard?>>> func = () => _exceptionHelper.Accessor.PostAsync(postLeaderboard);

            // Assert
            using (new AssertionScope())
            {
                var leaderboard = await func.Invoke();
                leaderboard?.Should().NotBeNull();
                leaderboard?.Result.Should().Be(ModelResult.Error);
                leaderboard?.Data?.Should().BeNull();
            }
        }
    }
}
