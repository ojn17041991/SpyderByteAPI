﻿using Microsoft.EntityFrameworkCore;
using SpyderByteAPI.Models.Games;
using SpyderByteAPI.Models.Jams;
using SpyderByteAPI.Models.Leaderboard;

namespace SpyderByteAPI.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Game> Games { get; set; }

        public DbSet<Jam> Jams { get; set; }

        public DbSet<LeaderboardRecord> LeaderboardRecords { get; set; }
    }
}
