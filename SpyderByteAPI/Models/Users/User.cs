using SpyderByteAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Users
{
    public class User
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Hash { get; set; } = string.Empty;

        [Required]
        public string Salt { get; set; } = string.Empty;

        [Required]
        public UserType UserType { get; set; } = UserType.Restricted;
    }
}
