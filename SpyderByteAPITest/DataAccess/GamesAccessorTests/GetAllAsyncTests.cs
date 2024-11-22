using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteTest.DataAccess.GamesAccessorTests.Helpers;
using SpyderByteDataAccess.Models.Games;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses.Abstract;
using System.Diagnostics;

namespace SpyderByteTest.DataAccess.GamesAccessorTests
{
    public class GetAllAsyncTests
    {
        private readonly GamesAccessorHelper _helper;
        private readonly GamesAccessorExceptionHelper _exceptionHelper;

        public GetAllAsyncTests()
        {
            _helper = new GamesAccessorHelper();
            _exceptionHelper = new GamesAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Get_Games_From_Accessor()
        {
            // Arrange
            var storedGame = await _helper.AddGame();

            // Act
            var returnedGames = await _helper.Accessor.GetAllAsync();

            // Assert
            using (new AssertionScope())
            {
                returnedGames.Should().NotBeNull();
                returnedGames.Result.Should().Be(ModelResult.OK);
                returnedGames.Data.Should().NotBeNull();
                returnedGames.Data.Should().HaveCount(1);
                returnedGames.Data.Should().ContainEquivalentOf
                (
                    storedGame,
                    options => options
                        .Excluding(g => g.LeaderboardGame)
                        .Excluding(g => g.UserGame)
                );
                returnedGames.Data!.Select(g => g.UserGame).Should().ContainEquivalentOf
                (
                    storedGame.UserGame!,
                    options => options
                        .Excluding(ug => ug.User)
                        .Excluding(ug => ug.Game)
                );
            }
        }

        [Fact]
        [Trait("Category", "Performance")]
        public async Task All_Games_Are_Returned_Within_Expected_Time_Frame()
        {
            // Arrange
            const int numGames = 1000;
            const int thresholdMs = 1000;

            _ = await _helper.AddGames(numGames);

            // Act
            Stopwatch stopWatch = Stopwatch.StartNew();
            var returnedGames = await _helper.Accessor.GetAllAsync();
            stopWatch.Stop();

            // Assert
            using (new AssertionScope())
            {
                returnedGames.Should().NotBeNull();
                returnedGames.Result.Should().Be(ModelResult.OK);
                returnedGames.Data.Should().NotBeNull();
                returnedGames.Data.Should().HaveCount(numGames);
                stopWatch.ElapsedMilliseconds.Should().BeLessThan(thresholdMs);
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange

            // Act
            Func<Task<IDataResponse<IList<Game>?>>> func = () => _exceptionHelper.Accessor.GetAllAsync();

            // Assert
            using (new AssertionScope())
            {
                var returnedGames = await func.Invoke();
                returnedGames?.Should().NotBeNull();
                returnedGames?.Result.Should().Be(ModelResult.Error);
                returnedGames?.Data?.Should().BeNull();
            }
        }
    }
}