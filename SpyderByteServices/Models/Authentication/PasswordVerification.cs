namespace SpyderByteServices.Models.Authentication
{
    public record PasswordVerification
    {
        public string Password { get; init; } = string.Empty;

        public string Hash { get; init; } = string.Empty;

        public string Salt { get; init; } = string.Empty;
    }
}
