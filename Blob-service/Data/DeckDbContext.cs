using Blob_service.Models;
using Microsoft.EntityFrameworkCore;

namespace Blob_service.Data
{
    public class DeckDbContext : DbContext
    {
        public DeckDbContext(DbContextOptions<DeckDbContext> options) : base(options)
        {
        }

        public DbSet<SixPlayerHand> Hands { get; set; }

        public DbSet<ActiveHand> ActiveHand { get; set; }

        public DbSet<Bids> Bids { get; set; }

        public DbSet<Scores> Scores { get; set; }

        public DbSet<GameDetails> GameDetails { get; set; }
    }
}
