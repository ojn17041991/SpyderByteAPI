using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPI.Enums;
using SpyderByteAPITest.DataAccess.GamesAccessorTests.Helper;

namespace SpyderByteAPITest.DataAccess.GamesAccessorTests
{
    public class GetAllAsyncTests
    {
        private readonly GamesAccessorHelper _helper;

        public GetAllAsyncTests()
        {
            _helper = new GamesAccessorHelper();
        }

        [Fact]
        public async Task Can_Get_Games_From_Accessor()
        {
            // Arrange
            var game = await _helper.AddGame();

            // Act
            var games = await _helper.Accessor.GetAllAsync();

            // Assert
            using (new AssertionScope())
            {
                games.Should().NotBeNull();
                games.Result.Should().Be(ModelResult.OK);
                games.Data.Should().NotBeNull();
                games.Data.Should().HaveCount(1);
                games.Data.Should().ContainEquivalentOf(game);
            }
        }

        // how can i trigger an exception in this test?
        [Fact]
        public async Task This_Is_Meant_To_Fail()
        {
            // Arrange

            // Act

            // Assert
            true.Should().BeFalse();
        }
    }
}