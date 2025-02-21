using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteTest.DataAccess.GamesAccessorTests.Helpers;
using SpyderByteDataAccess.Models.Games;
using SpyderByteResources.Enums;
using SpyderByteResources.Models.Responses.Abstract;
using System.Diagnostics;
using SpyderByteResources.Models.Paging.Abstract;

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

            string? name = null;
            GameType? type = null;
            int page = 1;
            int pageSize = 10;
            string? order = null;
            string? direction = null;

            // Act
            var returnedGames = await _helper.Accessor.GetAllAsync(name, type, page, pageSize, order, direction);

            // Assert
            using (new AssertionScope())
            {
                returnedGames.Should().NotBeNull();
                returnedGames.Result.Should().Be(ModelResult.OK);
                returnedGames.Data.Should().NotBeNull();
                returnedGames.Data!.Items.Should().HaveCount(1);
                returnedGames.Data!.Page.Should().Be(page);
                returnedGames.Data!.PageSize.Should().Be(pageSize);
                returnedGames.Data!.HasNextPage.Should().BeFalse();
                returnedGames.Data!.HasPreviousPage.Should().BeFalse();
                returnedGames.Data!.Items.Should().ContainEquivalentOf
                (
                    storedGame,
                    options => options
                        .Excluding(g => g.LeaderboardGame)
                        .Excluding(g => g.UserGame)
                );
                returnedGames.Data!.Items.Select(g => g.UserGame).Should().ContainEquivalentOf
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
        public async Task All_Games_Are_Returned_Within_Expected_Time_Frame_For_Small_Page_Size()
        {
            // Arrange
            const int numGames = 1000;
            const int thresholdMs = 1000;

            string? name = null;
            GameType? type = null;
            int page = 1;
            int pageSize = 10;
            string? order = null;
            string? direction = null;

            _ = await _helper.AddGames(numGames);

            // Act
            Stopwatch stopWatch = Stopwatch.StartNew();
            var returnedGames = await _helper.Accessor.GetAllAsync(name, type, page, pageSize, order, direction);
            stopWatch.Stop();

            // Assert
            using (new AssertionScope())
            {
                returnedGames.Should().NotBeNull();
                returnedGames.Result.Should().Be(ModelResult.OK);
                returnedGames.Data.Should().NotBeNull();
                returnedGames.Data!.Items.Should().HaveCount(numGames);
                stopWatch.ElapsedMilliseconds.Should().BeLessThan(thresholdMs);
            }
        }

        [Fact]
        [Trait("Category", "Performance")]
        public async Task All_Games_Are_Returned_Within_Expected_Time_Frame_For_Large_Page_Size()
        {
            // Arrange
            const int numGames = 1000;
            const int thresholdMs = 1000;

            string? name = null;
            GameType? type = null;
            int page = 1;
            int pageSize = 1000;
            string? order = null;
            string? direction = null;

            _ = await _helper.AddGames(numGames);

            // Act
            Stopwatch stopWatch = Stopwatch.StartNew();
            var returnedGames = await _helper.Accessor.GetAllAsync(name, type, page, pageSize, order, direction);
            stopWatch.Stop();

            // Assert
            using (new AssertionScope())
            {
                returnedGames.Should().NotBeNull();
                returnedGames.Result.Should().Be(ModelResult.OK);
                returnedGames.Data.Should().NotBeNull();
                returnedGames.Data!.Items.Should().HaveCount(numGames);
                stopWatch.ElapsedMilliseconds.Should().BeLessThan(thresholdMs);
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange
            string? name = null;
            GameType? type = null;
            int page = 1;
            int pageSize = 10;
            string? order = null;
            string? direction = null;

            // Act
            Func<Task<IDataResponse<IPagedList<Game>?>>> func = () => _exceptionHelper.Accessor.GetAllAsync(name, type, page, pageSize, order, direction);

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