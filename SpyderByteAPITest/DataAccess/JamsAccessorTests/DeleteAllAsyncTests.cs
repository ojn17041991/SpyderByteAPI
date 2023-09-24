using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Jams;
using SpyderByteAPITest.DataAccess.JamsAccessorTests.Helper;

namespace SpyderByteAPITest.DataAccess.JamsAccessorTests
{
    public class DeleteAllAsyncTests
    {
        private readonly JamsAccessorHelper _helper;
        private readonly JamsAccessorExceptionHelper _exceptionHelper;

        public DeleteAllAsyncTests()
        {
            _helper = new JamsAccessorHelper();
            _exceptionHelper = new JamsAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Delete_All_Jams_In_Accessor()
        {
            // Arrange
            var dbJam = await _helper.AddJam();

            // Act
            var jams = await _helper.Accessor.DeleteAllAsync();

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                jams.Should().NotBeNull();
                jams.Result.Should().Be(ModelResult.OK);
                jams.Data.Should().HaveCount(1);
                jams.Data!.First().Should().BeEquivalentTo(dbJam);

                // Check the database.
                var postTestJams = await _helper.GetJams();
                postTestJams.Should().HaveCount(0);
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange

            // Act
            Func<Task<IDataResponse<IList<Jam>?>>> func = () => _exceptionHelper.Accessor.DeleteAllAsync();

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
