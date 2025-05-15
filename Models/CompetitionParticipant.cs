using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoSurvey.Models
{
    public class CompetitionParticipant
    {
        [Key]
        public int ParticipantId { get; set; }

        public int CompetitionId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        public int Score { get; set; }

        public DateTime SubmissionDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("CompetitionId")]
        public virtual Competition Competition { get; set; }

        public virtual ICollection<CompetitionAnswer> Answers { get; set; } = new List<CompetitionAnswer>();
    }
}