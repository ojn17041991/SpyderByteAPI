using Microsoft.EntityFrameworkCore;
using SpyderByteDataAccess.Models.Games;
using SpyderByteDataAccess.Models.Leaderboards;
using SpyderByteDataAccess.Models.Users;

namespace SpyderByteDataAccess.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Game> Games { get; set; }

        public DbSet<LeaderboardGame> LeaderboardGames { get; set; }

        public DbSet<Leaderboard> Leaderboards { get; set; }

        public DbSet<LeaderboardRecord> LeaderboardRecords { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserGame> UserGames { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGame>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_UserGame_Id");

                entity.HasOne(e => e.User)
                    .WithOne(e => e.UserGame)
                    .HasForeignKey<UserGame>(e => e.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserGame_User");

                entity.HasOne(e => e.Game)
                    .WithOne(e => e.UserGame)
                    .HasForeignKey<UserGame>(e => e.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserGame_Game");
            });

            modelBuilder.Entity<LeaderboardGame>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_LeaderboardGame_Id");

                entity.HasOne(e => e.Game)
                    .WithOne(e => e.LeaderboardGame)
                    .HasForeignKey<LeaderboardGame>(e => e.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LeaderboardGame_Game");

                entity.HasOne(e => e.Leaderboard)
                    .WithOne(e => e.LeaderboardGame)
                    .HasForeignKey<LeaderboardGame>(e => e.LeaderboardId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LeaderboardGame_Leaderboard");
            });

            modelBuilder.Entity<LeaderboardRecord>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_LeaderboardRecord_Id");

                entity.HasOne(e => e.Leaderboard)
                    .WithMany(e => e.LeaderboardRecords)
                    .HasForeignKey(e => e.LeaderboardId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LeaderboardRecord_Leaderboard");
            });
        }
    }
}
