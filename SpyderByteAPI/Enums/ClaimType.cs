using System.ComponentModel;

namespace SpyderByteAPI.Enums
{
    public enum ClaimType
    {
        [Description("ReadGames")]
        ReadGames,

        [Description("WriteGames")]
        WriteGames,

        [Description("ReadJams")]
        ReadJams,

        [Description("WriteJams")]
        WriteJams,

        [Description("ReadLeaderboards")]
        ReadLeaderboards,

        [Description("WriteLeaderboards")]
        WriteLeaderboards,

        [Description("DeleteLeaderboards")]
        DeleteLeaderboards
    }
}
