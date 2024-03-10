using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPITest.DataAccess.LeaderboardAccessorTests.Helper;
using SpyderByteDataAccess.Models.Leaderboards;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses.Abstract;

namespace SpyderByteAPITest.DataAccess.LeaderboardAccessorTests
{
    public class GetAsyncTests
    {
        private readonly LeaderboardAccessorHelper _helper;
        private readonly LeaderboardAccessorExceptionHelper _exceptionHelper;

        public GetAsyncTests()
        {
            _helper = new LeaderboardAccessorHelper();
            _exceptionHelper = new LeaderboardAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Get_Leaderboard_Records_From_Accessor()
        {
            // Arrange
            var leaderboardRecord = await _helper.AddLeaderboardRecord();

            // Act
            var leaderboardRecords = await _helper.Accessor.GetAsync(leaderboardRecord.Id);

            // Assert
            using (new AssertionScope())
            {
                leaderboardRecords.Should().NotBeNull();
                leaderboardRecords.Result.Should().Be(ModelResult.OK);
                leaderboardRecords.Data.Should().NotBeNull();
                leaderboardRecords.Data.Should().BeEquivalentTo(leaderboardRecord);
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange

            // Act
            Func<Task<IDataResponse<Leaderboard?>>> func = () => _exceptionHelper.Accessor.GetAsync(Guid.NewGuid());

            // Assert
            using (new AssertionScope())
            {
                var leaderboardRecords = await func.Invoke();
                leaderboardRecords?.Should().NotBeNull();
                leaderboardRecords?.Result.Should().Be(ModelResult.Error);
                leaderboardRecords?.Data?.Should().BeNull();
            }
        }
    }
}
