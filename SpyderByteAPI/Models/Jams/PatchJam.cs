using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Jams
{
    public class PatchJam
    {
        [Required]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? ImgurUrl { get; set; }

        public string? ItchUrl { get; set; }
    }
}
