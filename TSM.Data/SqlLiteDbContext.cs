using Microsoft.EntityFrameworkCore;
using TSM.Data.Models;

namespace TSM.Data
{
    public class SqlLiteDbContext : DbContext
    {
        public SqlLiteDbContext(DbContextOptions options) : base(options)
        {
        }

        protected SqlLiteDbContext()
        {
        }

        public DbSet<CharacterAuctionSale> CharacterAuctionSales { get; set; }

        public DbSet<CharacterBank> CharacterBanks { get; set; }

        public DbSet<CharacterBuy> CharacterBuys { get; set; }

        public DbSet<CharacterCancelledAuction> CharacterCancelledAuctions { get; set; }

        public DbSet<CharacterExpiredAuction> CharacterExpiredAuctions { get; set; }

        public DbSet<CharacterInventory> CharacterInventories { get; set; }

        public DbSet<CharacterReagent> CharacterReagents { get; set; }

        public DbSet<Character> Characters { get; set; }

        public DbSet<Item> Items { get; set; }
    }
}