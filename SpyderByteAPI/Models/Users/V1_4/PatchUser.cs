namespace SpyderByteAPI.Models.Users.V1_4
{
    public record PatchUser
    {
        public Guid? GameId { get; init; }
    }
}
