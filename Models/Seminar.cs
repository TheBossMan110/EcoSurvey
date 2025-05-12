using System;
using System.ComponentModel.DataAnnotations;

namespace EcoSurvey.Models
{
    public class Seminar
    {
        [Key]
        public int SeminarId { get; set; }

        [Required]
        [StringLength(200)]
        public string SeminarTitle { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; }

        [Required]
        public DateTime ConductedDate { get; set; }

        [Required]
        public int NumberOfParticipants { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
