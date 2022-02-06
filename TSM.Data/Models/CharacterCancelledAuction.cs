using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSM.Data.Models
{
    [Table(nameof(CharacterCancelledAuction))]
    internal class CharacterCancelledAuction
    {
        [Required]
        public DateTime CancelledTime { get; set; }

        public virtual Character Character { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int CharacterCancelledAuctionID { get; set; }

        [Required]
        [ForeignKey(nameof(Character))]
        public int CharacterID { get; set; }

        [Required]
        [MaxLength(255)]
        public string ItemID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int StackSize { get; set; }
    }
}