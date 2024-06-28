using System.Text.RegularExpressions;

namespace SpyderByteResources.Helpers.Encoding
{
    public static class LogEncoder
    {
        private static Regex regex = new Regex("[<>\\\\\\/\\r\\n]");

        public static string Encode(string input)
        {
            string output = regex.Replace(input, string.Empty);
            return output;
        }
    }
}
