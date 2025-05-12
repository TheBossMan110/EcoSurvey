using System;
using System.ComponentModel.DataAnnotations;

namespace EcoSurvey.Models
{
    public class Winner
    {
        [Key]
        public int WinnerId { get; set; }

        public int CompetitionId { get; set; }

        [Required]
        [StringLength(100)]
        public string ParticipantName { get; set; }

        [Required]
        [StringLength(100)]
        public string ParticipantEmail { get; set; }

        [Required]
        public int Position { get; set; } // 1=1st, 2=2nd, etc.

        [Required]
        [StringLength(100)]
        public string Prize { get; set; }

        public DateTime AwardedDate { get; set; } = DateTime.Now;
    }
}
