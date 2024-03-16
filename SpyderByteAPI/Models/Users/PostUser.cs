using SpyderByteResources.Enums;
using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Users
{
    public class PostUser
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserType UserType { get; set; } = UserType.Restricted;

        public Guid? GameId { get; set; }
    }
}
