using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Authentication;
using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Users
{
    public class PostHashedUser
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        public HashData HashData { get; set; } = new HashData();

        [Required]
        public UserType UserType{ get; set; }
    }
}
