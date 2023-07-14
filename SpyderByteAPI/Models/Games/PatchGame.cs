using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Games
{
    public class PatchGame
    {
        [Required]
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}
