using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Authentication
{
    public class Login
    {
        [Required]
        [RegularExpression(@"[^<>\\\/\r\n]{1,50}", ErrorMessage = "UserName does not meet validation requirements.")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
