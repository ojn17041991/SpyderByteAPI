using FluentAssertions.Execution;
using FluentAssertions;
using SpyderByteAPI.Enums;
using SpyderByteAPITest.DataAccess.JamsAccessorTests.Helper;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Models.Jams;

namespace SpyderByteAPITest.DataAccess.JamsAccessorTests
{
    public class PostAsyncTests
    {
        private readonly JamsAccessorHelper _helper;
        private readonly JamsAccessorExceptionHelper _exceptionHelper;

        public PostAsyncTests()
        {
            _helper = new JamsAccessorHelper();
            _exceptionHelper = new JamsAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Add_Jam_To_Accessor()
        {
            // Arrange
            var preTestJams = await _helper.GetJams();
            var postJam = _helper.GeneratePostJam();

            // Act
            var jam = await _helper.Accessor.PostAsync(postJam);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                jam.Should().NotBeNull();
                jam.Result.Should().Be(ModelResult.Created);
                jam.Data.Should().NotBeNull();
                jam.Data.Should().BeEquivalentTo(postJam, options => options.Excluding(j => j.Image));
                jam.Data!.Id.Should().NotBeEmpty();

                // Check the database.
                var dbJam = await _helper.GetJam(jam.Data!.Id);
                dbJam.Should().NotBeNull();
                dbJam.Should().BeEquivalentTo(jam.Data);
                dbJam.Should().BeEquivalentTo(postJam, options => options.Excluding(j => j.Image));

                var postTestJams = await _helper.GetJams();
                postTestJams.Should().HaveCount(preTestJams.Count + 1);
            }
        }

        [Fact]
        public async Task Can_Not_Add_Duplicate_Jam_To_Accessor()
        {
            // Arrange
            var preTestJams = await _helper.GetJams();
            var postJam = _helper.GeneratePostJam();

            // Act
            var response1 = await _helper.Accessor.PostAsync(postJam);
            var response2 = await _helper.Accessor.PostAsync(postJam);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                response1.Should().NotBeNull();
                response1.Result.Should().Be(ModelResult.Created);
                response1.Data.Should().NotBeNull();

                response2.Should().NotBeNull();
                response2.Result.Should().Be(ModelResult.AlreadyExists);
                response2.Data.Should().NotBeNull();
                response2.Data.Should().BeEquivalentTo(response1.Data);

                // Check the database.
                var postTestJams = await _helper.GetJams();
                postTestJams.Should().HaveCount(preTestJams.Count + 1);
            }
        }

        [Fact]
        public async Task Can_Not_Add_Jam_With_Invalid_Image_To_Accessor()
        {
            // Arrange
            var preTestJams = await _helper.GetJams();
            var postJam = _helper.GeneratePostJam();
            postJam.Image = null;

            // Act
            var jam = await _helper.Accessor.PostAsync(postJam);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                jam.Should().NotBeNull();
                jam.Result.Should().Be(ModelResult.RequestDataIncomplete);
                jam.Data.Should().BeNull();

                // Check the database.
                var postTestJams = await _helper.GetJams();
                postTestJams.Should().HaveCount(preTestJams.Count);
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange
            var postJam = _helper.GeneratePostJam();

            // Act
            Func<Task<IDataResponse<Jam?>>> func = () => _exceptionHelper.Accessor.PostAsync(postJam);

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
