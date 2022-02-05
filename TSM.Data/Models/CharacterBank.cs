using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSM.Data.Models
{
    [Table(nameof(CharacterBank))]
    public class CharacterBank
    {
        public virtual Character Character { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int CharacterBankID { get; set; }

        [ForeignKey(nameof(Character))]
        [Required]
        public int CharacterID { get; set; }

        [Required]
        [MaxLength(255)]
        public string ItemID { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}