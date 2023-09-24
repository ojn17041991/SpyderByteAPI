using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Jams;
using SpyderByteAPITest.DataAccess.JamsAccessorTests.Helper;

namespace SpyderByteAPITest.DataAccess.JamsAccessorTests
{
    public class PatchAsyncTests
    {
        private readonly JamsAccessorHelper _helper;
        private readonly JamsAccessorExceptionHelper _exceptionHelper;

        public PatchAsyncTests()
        {
            _helper = new JamsAccessorHelper();
            _exceptionHelper = new JamsAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Patch_Jam_In_Accessor()
        {
            // Arrange
            var dbJam = await _helper.AddJam();
            var patchJam = _helper.GeneratePatchJam();
            patchJam.Id = dbJam.Id;
            patchJam.Image = null;

            // Act
            var jam = await _helper.Accessor.PatchAsync(patchJam);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                jam.Should().NotBeNull();
                jam.Result.Should().Be(ModelResult.OK);
                jam.Data.Should().NotBeNull();
                jam.Data.Should().BeEquivalentTo(patchJam, options => options.Excluding(g => g.Image));

                // Check the database.
                var updatedDbJam = await _helper.GetJam(patchJam.Id);
                updatedDbJam.Should().NotBeNull();
                updatedDbJam.Should().BeEquivalentTo(jam.Data);
                updatedDbJam!.Id.Should().Be(dbJam.Id);
                updatedDbJam!.Name.Should().NotBe(dbJam.Name);
                updatedDbJam!.ItchUrl.Should().NotBe(dbJam.ItchUrl);
                updatedDbJam!.ImgurUrl.Should().Be(dbJam.ImgurUrl);
                updatedDbJam!.ImgurImageId.Should().Be(dbJam.ImgurImageId);
                updatedDbJam!.PublishDate.Should().NotBe(dbJam.PublishDate);
            }
        }

        [Fact]
        public async Task Can_Patch_Jam_With_Updated_Image_In_Accessor()
        {
            // Arrange
            var dbJam = await _helper.AddJam();
            var patchJam = _helper.GeneratePatchJam();
            patchJam.Id = dbJam.Id;

            // Act
            var jam = await _helper.Accessor.PatchAsync(patchJam);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                jam.Should().NotBeNull();
                jam.Result.Should().Be(ModelResult.OK);
                jam.Data.Should().NotBeNull();
                jam.Data.Should().BeEquivalentTo(patchJam, options => options.Excluding(g => g.Image));

                // Check the database.
                var updatedDbJam = await _helper.GetJam(patchJam.Id);
                updatedDbJam.Should().NotBeNull();
                updatedDbJam.Should().BeEquivalentTo(jam.Data);
                updatedDbJam!.Id.Should().Be(dbJam.Id);
                updatedDbJam!.ImgurUrl.Should().NotBe(dbJam.ImgurUrl);
                updatedDbJam!.ImgurImageId.Should().NotBe(dbJam.ImgurImageId);
            }
        }

        [Fact]
        public async Task Cannot_Patch_Jam_That_Does_Not_Exist_In_Accessor()
        {
            // Arrange
            var patchJam = _helper.GeneratePatchJam();

            // Act
            var jam = await _helper.Accessor.PatchAsync(patchJam);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                jam.Should().NotBeNull();
                jam.Result.Should().Be(ModelResult.NotFound);
                jam.Data.Should().BeNull();
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange
            var patchJam = _helper.GeneratePatchJam();

            // Act
            Func<Task<IDataResponse<Jam?>>> func = () => _exceptionHelper.Accessor.PatchAsync(patchJam);

            // Assert
            using (new AssertionScope())
            {
                var jams = await func.Invoke();
                jams?.Should().NotBeNull();
                jams?.Result.Should().Be(ModelResult.Error);
                jams?.Data?.Should().BeNull();
            }
        }

    }
}
