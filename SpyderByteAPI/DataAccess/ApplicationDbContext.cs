using Microsoft.EntityFrameworkCore;
using SpyderByteAPI.Models;

namespace SpyderByteAPI.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    // This uses TPH (Table-Per-Hierarchy) mapping.
        //    // So Game and WebGame are stored in the same table.
        //    // There is a Shadow Property; "Discriminator" added.
        //    // This just checks if the discriminator is WebGame and returns only if it is.
        //    builder.Entity<WebGame>()
        //        .HasDiscriminator<string>("WebGame");
        //}

        public DbSet<Game> Games { get; set; }
        //public DbSet<WebGame> WebGames { get; set; }
    }
}
