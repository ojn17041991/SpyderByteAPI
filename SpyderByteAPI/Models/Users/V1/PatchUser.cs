using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Users.V1
{
    public record PatchUser
    {
        [Required]
        public Guid Id { get; set; }

        public Guid? GameId { get; set; }
    }
}
