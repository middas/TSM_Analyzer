using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSM.Data.Models
{
    [Table(nameof(CharacterMailItem))]
    internal class CharacterMailItem
    {
        public virtual Character Character { get; set; }

        [Required]
        [ForeignKey(nameof(Character))]
        public int CharacterID { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int CharacterMailItemID { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        [MaxLength(255)]
        public string ItemID { get; set; }
    }
}