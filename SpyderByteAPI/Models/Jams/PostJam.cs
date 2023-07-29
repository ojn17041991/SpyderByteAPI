using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Jams
{
    public class PostJam
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string ItchUrl { get; set; } = string.Empty;

        [Required]
        public IFormFile? Image { get; set; }

        [Required]
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;
    }
}
