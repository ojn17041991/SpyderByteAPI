using System.ComponentModel;

namespace SpyderByteResources.Enums
{
    public enum ClaimType
    {
        [Description("UserId")]
        UserId,

        [Description("ReadUsers")]
        ReadUsers,

        [Description("WriteUsers")]
        WriteUsers,

        [Description("WriteGames")]
        WriteGames,

        [Description("ReadLeaderboards")]
        ReadLeaderboards,

        [Description("WriteLeaderboards")]
        WriteLeaderboards,

        [Description("WriteLeaderboardRecords")]
        WriteLeaderboardRecords,

        [Description("DeleteLeaderboards")]
        DeleteLeaderboards,

        [Description("DeleteLeaderboardRecords")]
        DeleteLeaderboardRecords,

        [Description("DataBackup")]
        DataBackup,

        [Description("DataCleanup")]
        DataCleanup,

        [Description("AssignedGame")]
        AssignedGame
    }
}
