﻿using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPI.Enums;
using SpyderByteAPITest.DataAccess.GamesAccessorTests.Helper;

namespace SpyderByteAPITest.DataAccess.GamesAccessorTests
{
    public class GetSingleAsyncTests
    {
        private readonly GamesAccessorHelper _helper;

        public GetSingleAsyncTests()
        {
            _helper = new GamesAccessorHelper();
        }

        [Fact]
        public async Task Can_Get_Single_Game_From_Accessor()
        {
            // Arrange
            var game = await _helper.AddGame();

            // Act
            var games = await _helper.Accessor.GetSingleAsync(game.Id);

            // Assert
            using (new AssertionScope())
            {
                games.Should().NotBeNull();
                games.Result.Should().Be(ModelResult.OK);
                games.Data.Should().NotBeNull();
                games.Data.Should().BeEquivalentTo(game);
            }
        }

        [Fact]
        public async Task Can_Not_Get_Single_Game_From_Accessor_With_Invalid_Id()
        {
            // Arrange
            await _helper.AddGame();

            // Act
            var games = await _helper.Accessor.GetSingleAsync(Guid.NewGuid());

            // Assert
            using (new AssertionScope())
            {
                games.Should().NotBeNull();
                games.Result.Should().Be(ModelResult.NotFound);
                games.Data.Should().BeNull();
            }
        }
    }
}