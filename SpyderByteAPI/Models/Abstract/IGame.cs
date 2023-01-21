namespace SpyderByteAPI.Models.Abstract
{
    public interface IGame
    {
        int? Id { get; set; }

        string? Name { get; set; }

        DateTime? PublishDate { get; set; }
    }
}
