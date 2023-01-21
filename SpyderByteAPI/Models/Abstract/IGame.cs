using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Abstract
{
    public interface IGame
    {
        [SwaggerSchema(ReadOnly = true)]
        int? Id { get; set; }

        [Required]
        string Name { get; set; }

        [Required]
        DateTime PublishDate { get; set; }
    }
}
