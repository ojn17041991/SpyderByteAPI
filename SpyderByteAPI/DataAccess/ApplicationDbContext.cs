using Microsoft.EntityFrameworkCore;
using SpyderByteAPI.Models.Games;
using SpyderByteAPI.Models.Jams;
using SpyderByteAPI.Models.Leaderboard;
using SpyderByteAPI.Models.Users;

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

        public DbSet<User> Users { get; set; }

        public DbSet<UserJam> UserJams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserJam>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_UserJam_Id");

                entity.HasOne(e => e.User)
                    .WithOne(e => e.UserJam)
                    .HasForeignKey<UserJam>(e => e.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserJam_User");

                entity.HasOne(e => e.Jam)
                    .WithOne(e => e.UserJam)
                    .HasForeignKey<UserJam>(e => e.JamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserJam_Jam");
            });
        }
    }
}
