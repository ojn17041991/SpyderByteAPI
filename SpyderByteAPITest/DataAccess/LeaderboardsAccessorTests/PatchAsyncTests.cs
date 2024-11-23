using FluentAssertions.Execution;
using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.DataAccess.LeaderboardsAccessorTests.Helpers;
using SpyderByteResources.Responses.Abstract;
using SpyderByteDataAccess.Models.Leaderboards;

namespace SpyderByteTest.DataAccess.LeaderboardsAccessorTests
{
    public class PatchAsyncTests
    {
        private readonly LeaderboardsAccessorHelper _helper;
        private readonly LeaderboardsAccessorExceptionHelper _exceptionHelper;

        public PatchAsyncTests()
        {
            _helper = new LeaderboardsAccessorHelper();
            _exceptionHelper = new LeaderboardsAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Patch_Leaderboard_In_Accessor()
        {
            // Arrange
            var storedGame = await _helper.AddGameWithoutLeaderboard();
            var storedLeaderboard = await _helper.AddLeaderboardWithoutRecords();
            var patchLeaderboard = _helper.GeneratePatchLeaderboard();
            patchLeaderboard.GameId = storedGame.Id;
            patchLeaderboard.Id = storedLeaderboard.Id;

            // Act
            var returnedLeaderboard = await _helper.Accessor.PatchAsync(patchLeaderboard);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedLeaderboard.Should().NotBeNull();
                returnedLeaderboard.Result.Should().Be(ModelResult.OK);
                returnedLeaderboard.Data.Should().NotBeNull();
                returnedLeaderboard.Data!.LeaderboardGame.LeaderboardId.Should().Be(patchLeaderboard.Id);
                returnedLeaderboard.Data!.LeaderboardGame.GameId.Should().Be(patchLeaderboard.GameId);

                // Check the database.
                var updatedStoredLeaderboard = await _helper.GetLeaderboard(returnedLeaderboard.Data!.Id);
                updatedStoredLeaderboard.Should().NotBeNull();
                updatedStoredLeaderboard!.Id.Should().Be(returnedLeaderboard.Data!.Id);
                updatedStoredLeaderboard!.LeaderboardRecords.Should().BeEquivalentTo(returnedLeaderboard.Data!.LeaderboardRecords);
                updatedStoredLeaderboard!.LeaderboardGame.GameId.Should().Be(patchLeaderboard.GameId);
            }
        }

        [Fact]
        public async Task Can_Not_Patch_Leaderboard_In_Accessor_If_Leaderboard_Does_Not_Exist()
        {
            // Arrange
            var patchLeaderboard = _helper.GeneratePatchLeaderboard();

            // Act
            var returnedLeaderboard = await _helper.Accessor.PatchAsync(patchLeaderboard);

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
        public async Task Can_Not_Patch_Leaderboard_In_Accessor_If_Game_Does_Not_Exist()
        {
            // Arrange
            var storedGame = await _helper.AddGameWithoutLeaderboard();
            var storedLeaderboard = await _helper.AddLeaderboardWithoutRecords();
            var patchLeaderboard = _helper.GeneratePatchLeaderboard();
            patchLeaderboard.GameId = Guid.NewGuid();
            patchLeaderboard.Id = storedLeaderboard.Id;

            // Act
            var returnedLeaderboard = await _helper.Accessor.PatchAsync(patchLeaderboard);

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
        public async Task Can_Not_Patch_Leaderboard_In_Accessor_With_Game_That_Already_Has_A_Leaderboard()
        {
            // Arrange
            var storedGame1 = await _helper.AddGameWithLeaderboard();
            var storedGame2 = await _helper.AddGameWithLeaderboard();
            var patchLeaderboard = _helper.GeneratePatchLeaderboard();
            patchLeaderboard.Id = storedGame1.LeaderboardGame!.LeaderboardId;
            patchLeaderboard.GameId = storedGame2.LeaderboardGame!.GameId;

            // Act
            var returnedLeaderboard = await _helper.Accessor.PatchAsync(patchLeaderboard);

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
        public async Task Can_Not_Patch_Leaderboard_In_Accessor_If_Leaderboard_Is_Not_Related_To_Any_Existing_Game()
        {
            // Arrange
            var storedGame = await _helper.AddGameWithoutLeaderboard();
            var storedLeaderboard = await _helper.AddLeaderboardWithoutGame();
            var patchLeaderboard = _helper.GeneratePatchLeaderboard();
            patchLeaderboard.GameId = storedGame.Id;
            patchLeaderboard.Id = storedLeaderboard.Id;

            // Act
            var returnedLeaderboard = await _helper.Accessor.PatchAsync(patchLeaderboard);

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
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange
            var patchLeaderboard = _helper.GeneratePatchLeaderboard();

            // Act
            Func<Task<IDataResponse<Leaderboard?>>> func = () => _exceptionHelper.Accessor.PatchAsync(patchLeaderboard);

            // Assert
            using (new AssertionScope())
            {
                var games = await func.Invoke();
                games?.Should().NotBeNull();
                games?.Result.Should().Be(ModelResult.Error);
                games?.Data?.Should().BeNull();
            }
        }
    }
}
