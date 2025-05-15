// Models/Competition.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoSurvey.Models
{
    public class Competition
    {
        [Key]
        public int CompetitionId { get; set; }

        [Display(Name = "Associated Survey")]
        public int? SurveyId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Additional fields
        public string Rules { get; set; }
        public string Prizes { get; set; }

        [Display(Name = "Max Participants")]
        public int? MaxParticipants { get; set; }

        // Navigation property
        [ForeignKey("SurveyId")]
        public virtual Survey Survey { get; set; }

        // Add these new navigation properties
        public virtual ICollection<Winner> Winners { get; set; } = new List<Winner>();
        public virtual ICollection<CompetitionParticipant> Participants { get; set; } = new List<CompetitionParticipant>();
    }
}