using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Authentication
{
    public record Login
    {
        [Required]
        [RegularExpression(@"[^<>\\\/\r\n]{1,50}", ErrorMessage = "UserName does not meet validation requirements.")]
        public string UserName { get; init; } = string.Empty;

        [Required]
        public string Password { get; init; } = string.Empty;
    }
}
