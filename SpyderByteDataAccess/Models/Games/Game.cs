﻿using Microsoft.EntityFrameworkCore;
using SpyderByteDataAccess.Models.Leaderboards;
using SpyderByteDataAccess.Models.Users;
using SpyderByteResources.Enums;

namespace SpyderByteDataAccess.Models.Games
{
    [Index(nameof(Type))]
    public class Game
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public GameType Type { get; set; }

        public string Url { get; set; } = string.Empty;

        public string ImgurUrl { get; set; } = string.Empty;

        public string ImgurImageId { get; set; } = string.Empty;

        public DateTime PublishDate { get; set; } = DateTime.MinValue;

        public LeaderboardGame? LeaderboardGame { get; set; } = null!;

        public UserGame? UserGame { get; set; } = null!;
    }
}
