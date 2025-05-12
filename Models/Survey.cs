using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoSurvey.Models
{
    public class Survey
    {
        [Key]
        public int SurveyId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public byte TargetUserType { get; set; } // 2=Faculty/Staff, 3=Student

        [NotMapped]
        public string TargetUserTypeDisplay => TargetUserType switch
        {
            0 => "All",
            2 => "Faculty/Staff",
            3 => "Student",
            _ => "Unknown" // Handles unexpected values
        };

        public enum UserType : byte
        {
            All = 0,
            FacultyStaff = 2,  // Matches your comment
            Student = 3       // Matches your comment
        }

        [Range(0, 1440, ErrorMessage = "Estimated time must be between 0 and 1440 minutes (24 hours)")]
        [Display(Name = "Estimated Completion Time (minutes)")]
        public int EstimatedCompletionTime { get; set; } = 0;  // Default to 0 (not specified)

        [NotMapped]
        public IEnumerable<Survey> Surveys { get; set; }

        [NotMapped]
        public Dictionary<int, int> ResponseCounts { get; set; } = new Dictionary<int, int>();
    }
}