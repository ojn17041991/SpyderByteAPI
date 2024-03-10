namespace SpyderByteServices.Models.Authentication
{
    public class PasswordVerification
    {
        public string Password { get; set; } = string.Empty;

        public string Hash { get; set; } = string.Empty;

        public string Salt { get; set; } = string.Empty;
    }
}
