using System;
using System.ComponentModel.DataAnnotations;

namespace EcoSurvey.Models
{
    public class EffectiveParticipation
    {
        [Key]
        public int ParticipationId { get; set; }

        [Required]
        [StringLength(100)]
        public string ParticipantName { get; set; }

        [Required]
        [StringLength(100)]
        public string ParticipantEmail { get; set; }

        public int SeminarId { get; set; }

        public DateTime ParticipationDate { get; set; } = DateTime.Now;
    }
}
