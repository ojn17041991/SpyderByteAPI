﻿using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPI.Enums;
using SpyderByteAPITest.DataAccess.GamesAccessorTests.Helper;

namespace SpyderByteAPITest.DataAccess.GamesAccessorTests
{
    public class PatchAsyncTests
    {
        private readonly GamesAccessorHelper _helper;

        public PatchAsyncTests()
        {
            _helper = new GamesAccessorHelper();
        }

        [Fact]
        public async Task Can_Patch_Game_In_Accessor()
        {
            // Arrange
            var dbGame = await _helper.AddGame();
            var patchGame = _helper.GeneratePatchGame();
            patchGame.Id = dbGame.Id;
            patchGame.Image = null;

            // Act
            var game = await _helper.Accessor.PatchAsync(patchGame);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                game.Should().NotBeNull();
                game.Result.Should().Be(ModelResult.OK);
                game.Data.Should().NotBeNull();
                game.Data.Should().BeEquivalentTo(patchGame, options => options.Excluding(g => g.Image));

                // Check the database.
                var updatedDbGame = await _helper.GetGame(patchGame.Id);
                updatedDbGame.Should().NotBeNull();
                updatedDbGame.Should().BeEquivalentTo(game.Data);
                updatedDbGame!.Id.Should().Be(dbGame.Id);
                updatedDbGame!.Name.Should().NotBe(dbGame.Name);
                updatedDbGame!.HtmlUrl.Should().NotBe(dbGame.HtmlUrl);
                updatedDbGame!.ImgurUrl.Should().Be(dbGame.ImgurUrl);
                updatedDbGame!.ImgurImageId.Should().Be(dbGame.ImgurImageId);
                updatedDbGame!.PublishDate.Should().NotBe(dbGame.PublishDate);
            }
        }

        [Fact]
        public async Task Can_Patch_Game_With_Updated_Image_In_Accessor()
        {
            // Arrange
            var dbGame = await _helper.AddGame();
            var patchGame = _helper.GeneratePatchGame();
            patchGame.Id = dbGame.Id;

            // Act
            var game = await _helper.Accessor.PatchAsync(patchGame);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                game.Should().NotBeNull();
                game.Result.Should().Be(ModelResult.OK);
                game.Data.Should().NotBeNull();
                game.Data.Should().BeEquivalentTo(patchGame, options => options.Excluding(g => g.Image));

                // Check the database.
                var updatedDbGame = await _helper.GetGame(patchGame.Id);
                updatedDbGame.Should().NotBeNull();
                updatedDbGame.Should().BeEquivalentTo(game.Data);
                updatedDbGame!.Id.Should().Be(dbGame.Id);
                updatedDbGame!.ImgurUrl.Should().NotBe(dbGame.ImgurUrl);
                updatedDbGame!.ImgurImageId.Should().NotBe(dbGame.ImgurImageId);
            }
        }

        [Fact]
        public async Task Can_Not_Patch_Game_That_Does_Not_Exist_In_Accessor()
        {
            // Arrange
            var patchGame = _helper.GeneratePatchGame();

            // Act
            var game = await _helper.Accessor.PatchAsync(patchGame);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                game.Should().NotBeNull();
                game.Result.Should().Be(ModelResult.NotFound);
                game.Data.Should().BeNull();
            }
        }
    }
}