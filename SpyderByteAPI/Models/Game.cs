using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using SpyderByteAPI.Models.Abstract;

namespace SpyderByteAPI.Models
{
    public class Game : IGame
    {
        [SwaggerSchema(ReadOnly = true)]
        public int? Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime PublishDate { get; set; }
    }
}
