using Swashbuckle.AspNetCore.Annotations;

namespace SpyderByteAPI.Models.Leaderboard
{
    public class Leaderboard
    {
        [SwaggerSchema(ReadOnly = true)]
        public int? Id { get; set; }

        public string? Name { get; set; }

        public DateTime? PublishDate { get; set; }
    }
}