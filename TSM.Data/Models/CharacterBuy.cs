using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSM.Data.Models
{
    [Table(nameof(CharacterBuy))]
    internal class CharacterBuy
    {
        [Required]
        public DateTime BoughtTime { get; set; }

        public virtual Character Character { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int CharacterAuctionBuyID { get; set; }

        [ForeignKey(nameof(Character))]
        [Required]
        public int CharacterID { get; set; }

        [Required]
        public long Copper { get; set; }

        [Required]
        [MaxLength(255)]
        public string ItemID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [MaxLength(255)]
        public string Source { get; set; }

        [Required]
        public int StackSize { get; set; }
    }
}