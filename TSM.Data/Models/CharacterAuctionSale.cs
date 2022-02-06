using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSM.Data.Models
{
    [Table(nameof(CharacterAuctionSale))]
    internal class CharacterAuctionSale
    {
        public virtual Character Character { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int CharacterAuctionSaleID { get; set; }

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
        public DateTime TimeOfSale { get; set; }
    }
}