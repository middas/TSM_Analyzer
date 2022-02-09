using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSM.Data.Models
{
    [Table(nameof(Character))]
    internal class Character
    {
        public virtual ICollection<CharacterAuctionSale> CharacterAuctionSales { get; set; }

        public virtual ICollection<CharacterBank> CharacterBankItems { get; set; }

        public virtual ICollection<CharacterBuy> CharacterBuys { get; set; }

        public virtual ICollection<CharacterCancelledAuction> CharacterCancelledAuctions { get; set; }

        public virtual ICollection<CharacterExpiredAuction> CharacterExpiredAuctions { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int CharacterID { get; set; }

        public virtual ICollection<CharacterInventory> CharacterInventoryItems { get; set; }

        public virtual ICollection<CharacterMailItem> CharacterMailItems { get; set; }

        public virtual ICollection<CharacterReagent> CharacterReagents { get; set; }

        [Required]
        [MaxLength(255)]
        public string Class { get; set; }

        [Required]
        public long Copper { get; set; }

        [Required]
        [MaxLength(255)]
        public string Faction { get; set; }

        [Required]
        public DateTime LastUpdateTime { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Realm { get; set; }
    }
}