using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Jams;
using SpyderByteAPITest.DataAccess.JamsAccessorTests.Helper;

namespace SpyderByteAPITest.DataAccess.JamsAccessorTests
{
    public class DeleteAsyncTests
    {
        private readonly JamsAccessorHelper _helper;
        private readonly JamsAccessorExceptionHelper _exceptionHelper;

        public DeleteAsyncTests()
        {
            _helper = new JamsAccessorHelper();
            _exceptionHelper = new JamsAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Delete_Jam_In_Accessor()
        {
            // Arrange
            var dbJam = await _helper.AddJam();
            var preTestJams = await _helper.GetJams();

            // Act
            var jam = await _helper.Accessor.DeleteAsync(dbJam.Id);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                jam.Should().NotBeNull();
                jam.Result.Should().Be(ModelResult.OK);
                jam.Data.Should().BeEquivalentTo(dbJam);

                // Check the database.
                var postTestJams = await _helper.GetJams();
                postTestJams.Should().HaveCount(preTestJams.Count - 1);
            }
        }

        [Fact]
        public async Task Can_Not_Delete_Jam_That_Does_Not_Exist_In_Accessor()
        {
            // Arrange
            var preTestJams = await _helper.GetJams();

            // Act
            var jam = await _helper.Accessor.DeleteAsync(Guid.NewGuid());

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                jam.Should().NotBeNull();
                jam.Result.Should().Be(ModelResult.NotFound);
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
            var dbJam = await _helper.AddJam();

            // Act
            Func<Task<IDataResponse<Jam?>>> func = () => _exceptionHelper.Accessor.DeleteAsync(dbJam.Id);

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
