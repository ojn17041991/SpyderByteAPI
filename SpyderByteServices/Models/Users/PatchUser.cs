namespace SpyderByteServices.Models.Users
{
    public record PatchUser
    {
        public Guid Id { get; set; }

        public Guid? GameId { get; set; }
    }
}
