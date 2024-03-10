namespace SpyderByteServices.Models.Authentication
{
    public class HashData
    {
        public string Hash { get; set; } = string.Empty;

        public string Salt { get; set; } = string.Empty;

        public char Pepper { get; set; }
    }
}
