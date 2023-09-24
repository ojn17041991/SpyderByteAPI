using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Jams;
using SpyderByteAPITest.DataAccess.JamsAccessorTests.Helper;

namespace SpyderByteAPITest.DataAccess.JamsAccessorTests
{
    public class GetSingleAsyncTests
    {
        private readonly JamsAccessorHelper _helper;
        private readonly JamsAccessorExceptionHelper _exceptionHelper;

        public GetSingleAsyncTests()
        {
            _helper = new JamsAccessorHelper();
            _exceptionHelper = new JamsAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Get_Single_Jam_From_Accessor()
        {
            // Arrange
            var jam = await _helper.AddJam();

            // Act
            var jams = await _helper.Accessor.GetSingleAsync(jam.Id);

            // Assert
            using (new AssertionScope())
            {
                jams.Should().NotBeNull();
                jams.Result.Should().Be(ModelResult.OK);
                jams.Data.Should().NotBeNull();
                jams.Data.Should().BeEquivalentTo(jam);
            }
        }

        [Fact]
        public async Task Can_Not_Get_Single_Jam_From_Accessor_With_Invalid_Id()
        {
            // Arrange
            await _helper.AddJam();

            // Act
            var jams = await _helper.Accessor.GetSingleAsync(Guid.NewGuid());

            // Assert
            using (new AssertionScope())
            {
                jams.Should().NotBeNull();
                jams.Result.Should().Be(ModelResult.NotFound);
                jams.Data.Should().BeNull();
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange

            // Act
            Func<Task<IDataResponse<Jam?>>> func = () => _exceptionHelper.Accessor.GetSingleAsync(Guid.NewGuid());

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
