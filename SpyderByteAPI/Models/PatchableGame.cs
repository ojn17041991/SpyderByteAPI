using SpyderByteAPI.Models.Abstract;
using Swashbuckle.AspNetCore.Annotations;

namespace SpyderByteAPI.Models
{
    public class PatchableGame : IGame, IPatchable
    {
        [SwaggerSchema(ReadOnly = true)]
        public int? Id { get; set; }

        public string? Name { get; set; }

        public DateTime? PublishDate { get; set; }
    }
}
