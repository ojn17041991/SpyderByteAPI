namespace SpyderByteAPI.Helpers.Authorization
{
    public static class PolicyType
    {
        // OJN: Feels redundant to have policies and claims that are basically the same thing...
        public const string ReadGames = "ReadGames";
        public const string WriteGames = "WriteGames";
        public const string ReadJams = "ReadJams";
        public const string WriteJams = "WriteJams";
        public const string ReadLeaderboards = "ReadLeaderboards";
        public const string WriteLeaderboards = "WriteLeaderboards";
        public const string DeleteLeaderboards = "DeleteLeaderboards";
    }
}
