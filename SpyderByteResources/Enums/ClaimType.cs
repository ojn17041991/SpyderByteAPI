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

        [Description("WriteJams")]
        WriteJams,

        [Description("ReadLeaderboards")]
        ReadLeaderboards,

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
