namespace SpyderByteResources.Helpers.Authorization
{
    public class TokenBlacklister
    {
        public static int MaxTokensStored { get; } = 100;

        private static Queue<string> blacklist = new Queue<string>();

        public static void AddTokenToBlacklist(string token)
        {
            blacklist.Enqueue(token);

            if (blacklist.Count > MaxTokensStored)
            {
                _ = blacklist.Dequeue();
            }
        }

        public static void ClearBlacklist()
        {
            blacklist.Clear(); 
        }

        public static IList<string> GetBlacklistedTokens()
        {
            return blacklist.ToList();
        }

        public static bool IsTokenBlacklisted(string token)
        {
            return blacklist.Contains(token);
        }
    }
}
