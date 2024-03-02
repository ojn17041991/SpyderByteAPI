using SpyderByteAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Users
{
    public class PostUser
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserType UserType { get; set; } = UserType.Restricted;

        public Guid? JamId { get; set; }
    }
}
