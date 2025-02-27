﻿using Microsoft.EntityFrameworkCore;

namespace SpyderByteDataAccess.Models.Leaderboards
{
    [Index(nameof(LeaderboardId))]
    public class LeaderboardRecord
    {
        public Guid Id { get; set; }

        public Guid LeaderboardId { get; set; }

        public Leaderboard Leaderboard { get; set; } = null!;

        public string Player { get; set; } = string.Empty;

        public long Score { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
