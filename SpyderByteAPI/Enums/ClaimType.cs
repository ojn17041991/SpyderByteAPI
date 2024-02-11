using System.ComponentModel;

namespace SpyderByteAPI.Enums
{
    public enum ClaimType
    {
        [Description("WriteGames")]
        WriteGames,

        [Description("WriteJams")]
        WriteJams,

        [Description("WriteLeaderboards")]
        WriteLeaderboards,

        [Description("DeleteLeaderboards")]
        DeleteLeaderboards,

        [Description("DataBackup")]
        DataBackup
    }
}
