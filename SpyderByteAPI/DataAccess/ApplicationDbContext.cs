using Microsoft.EntityFrameworkCore;
using SpyderByteAPI.Models;

namespace SpyderByteAPI.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Game> Games { get; set; }
        //public DbSet<WebGame> WebGames { get; set; }
    }
}
