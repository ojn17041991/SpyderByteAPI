using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Leaderboard;
using SpyderByteAPITest.DataAccess.LeaderboardAccessorTests.Helper;

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

        //[Fact]
        //public async Task Can_Get_Leaderboard_Records_From_Accessor()
        //{
        //    // Arrange
        //    var leaderboardRecord = await _helper.AddLeaderboardRecord();

        //    // Act
        //    var leaderboardRecords = await _helper.Accessor.GetAsync(leaderboardRecord.GameId);

        //    // Assert
        //    using (new AssertionScope())
        //    {
        //        leaderboardRecords.Should().NotBeNull();
        //        leaderboardRecords.Result.Should().Be(ModelResult.OK);
        //        leaderboardRecords.Data.Should().NotBeNull();
        //        leaderboardRecords.Data.Should().HaveCount(1);
        //        leaderboardRecords.Data.Should().ContainEquivalentOf(leaderboardRecord);
        //    }
        //}

        //[Fact]
        //public async Task Exceptions_Are_Caught_And_Handled()
        //{
        //    // Arrange

        //    // Act
        //    Func<Task<IDataResponse<IList<LeaderboardRecord>?>>> func = () => _exceptionHelper.Accessor.GetAsync(Guid.NewGuid());

        //    // Assert
        //    using (new AssertionScope())
        //    {
        //        var leaderboardRecords = await func.Invoke();
        //        leaderboardRecords?.Should().NotBeNull();
        //        leaderboardRecords?.Result.Should().Be(ModelResult.Error);
        //        leaderboardRecords?.Data?.Should().BeNull();
        //    }
        //}
    }
}
