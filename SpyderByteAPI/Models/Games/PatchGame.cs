using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Games
{
    public class PatchGame
    {
        [Required]
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? HtmlUrl { get; set; }

        public IFormFile? Image { get; set; }

        public DateTime? PublishDate { get; set; }
    }
}
