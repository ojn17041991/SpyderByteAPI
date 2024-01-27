using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Auth
{
    public class Authentication
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Secret { get; set; } = string.Empty;
    }
}
