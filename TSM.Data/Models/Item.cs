using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSM.Data.Models
{
    [Table(nameof(Item))]
    internal class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [MaxLength(255)]
        public string ItemID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}