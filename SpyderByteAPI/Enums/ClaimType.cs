using System.ComponentModel;

namespace SpyderByteAPI.Enums
{
    public enum ClaimType
    {
        [Description("ReadUsers")]
        ReadUsers,

        [Description("WriteUsers")]
        WriteUsers,

        [Description("WriteGames")]
        WriteGames,

        [Description("WriteJams")]
        WriteJams,

        [Description("WriteLeaderboards")]
        WriteLeaderboards,

        [Description("DeleteLeaderboards")]
        DeleteLeaderboards,

        [Description("DataBackup")]
        DataBackup,

        [Description("AssignedGame")]
        AssignedGame
    }
}
