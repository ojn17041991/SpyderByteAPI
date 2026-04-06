namespace SpyderByteServices.Models.Authentication
{
    public record HashData
    {
        public string Hash { get; init; } = string.Empty;

        public string Salt { get; init; } = string.Empty;

        public char Pepper { get; init; }
    }
}
