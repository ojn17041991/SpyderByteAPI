namespace SpyderByteAPI.Helpers.Authorization
{
    public class TokenBlacklister
    {
        private static int MAX_TOKENS_STORED = 100;
        private static HashSet<string> blacklist = new HashSet<string>();

        public static void AddTokenToBlacklist(string token)
        {
            blacklist.Add(token);
            if (blacklist.Count > MAX_TOKENS_STORED)
            {
                blacklist.Remove(blacklist.First());
            }
        }

        public static bool IsTokenBlacklisted(string token)
        {
            return blacklist.Contains(token);
        }
    }
}
