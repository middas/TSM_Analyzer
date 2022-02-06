using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSM.Data.Models
{
    [Table(nameof(ScannedBackup))]
    internal class ScannedBackup
    {
        [Required]
        [MaxLength(500)]
        public string BackupPath { get; set; }

        [Required]
        public double Duration { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int ScannedBackupID { get; set; }

        [Required]
        public DateTime ScannedTime { get; set; }
    }
}