using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Jams;
using SpyderByteAPITest.DataAccess.JamsAccessorTests.Helper;

namespace SpyderByteAPITest.DataAccess.JamsAccessorTests
{
    public class GetAllAsyncTests
    {
        private readonly JamsAccessorHelper _helper;
        private readonly JamsAccessorExceptionHelper _exceptionHelper;

        public GetAllAsyncTests()
        {
            _helper = new JamsAccessorHelper();
            _exceptionHelper = new JamsAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Get_Jams_From_Accessor()
        {
            // Arrange
            var jam = await _helper.AddJam();

            // Act
            var jams = await _helper.Accessor.GetAllAsync();

            // Assert
            using (new AssertionScope())
            {
                jams.Should().NotBeNull();
                jams.Result.Should().Be(ModelResult.OK);
                jams.Data.Should().NotBeNull();
                jams.Data.Should().HaveCount(1);
                jams.Data.Should().ContainEquivalentOf(jam);
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange

            // Act
            Func<Task<IDataResponse<IList<Jam>?>>> func = () => _exceptionHelper.Accessor.GetAllAsync();

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
