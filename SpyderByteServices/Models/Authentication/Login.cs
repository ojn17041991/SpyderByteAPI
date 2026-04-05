namespace SpyderByteServices.Models.Authentication
{
    public record Login
    {
        public string UserName { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;
    }
}
