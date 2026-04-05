using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Users.V1
{
    public record PatchUser
    {
        [Required]
        public Guid Id { get; init; }

        public Guid? GameId { get; init; }
    }
}
