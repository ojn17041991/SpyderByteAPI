namespace SpyderByteAPI.Helpers.Authorization
{
    public class TokenBlacklister
    {
        // OJN: Could have faster lookups with HashSet.

        private static int MAX_TOKENS_STORED = 100;
        private static string[] blacklist = new string[MAX_TOKENS_STORED];
        private static int blacklistIndex = 0;

        public static void AddTokenToBlacklist(string token)
        {
            blacklist[blacklistIndex] = token;
            blacklistIndex = ++blacklistIndex % MAX_TOKENS_STORED;
        }

        public static bool IsTokenBlacklisted(string token)
        {
            return blacklist.Contains(token);
        }
    }
}
