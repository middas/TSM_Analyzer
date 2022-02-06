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

        internal DbSet<CharacterAuctionSale> CharacterAuctionSales { get; set; }

        internal DbSet<CharacterBank> CharacterBanks { get; set; }

        internal DbSet<CharacterBuy> CharacterBuys { get; set; }

        internal DbSet<CharacterCancelledAuction> CharacterCancelledAuctions { get; set; }

        internal DbSet<CharacterExpiredAuction> CharacterExpiredAuctions { get; set; }

        internal DbSet<CharacterInventory> CharacterInventories { get; set; }

        internal DbSet<CharacterMailItem> CharacterMailItems { get; set; }

        internal DbSet<CharacterReagent> CharacterReagents { get; set; }

        internal DbSet<Character> Characters { get; set; }

        internal DbSet<Item> Items { get; set; }

        internal DbSet<ScannedBackup> ScannedBackups { get; set; }
    }
}