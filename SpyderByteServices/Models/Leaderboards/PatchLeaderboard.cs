namespace SpyderByteServices.Models.Leaderboards
{
    public record PatchLeaderboard
    {
        public Guid Id { get; set; }

        public Guid GameId { get; set; }
    }
}
