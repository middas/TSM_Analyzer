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

        public static explicit operator Core.Models.Character(Character c) => new(c.Name, Enum.Parse<Core.Models.Faction>(c.Faction, true), c.Realm)
        {
            Class = c.Class,
            Money = c.Copper,
            BagItems = c.CharacterInventoryItems.ToDictionary(x => x.ItemID, x => x.Quantity),
            BankItems = c.CharacterBankItems.ToDictionary(x => x.ItemID, x => x.Quantity),
            MailItems = c.CharacterMailItems.ToDictionary(x => x.ItemID, x => x.Count),
            ReagentItems = c.CharacterReagents.ToDictionary(x => x.ItemID, x => x.Quantity)
        };
    }
}