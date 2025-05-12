using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoSurvey.Models
{
    public class SurveyResult
    {
        [Key]
        public int ResultId { get; set; }

        public int SurveyId { get; set; }

        public int ResponseId { get; set; }

        [Required]
        public int TotalQuestions { get; set; }

        [Required]
        public int QuestionsAttempted { get; set; }

        [Required]
        public int CorrectAnswers { get; set; }

        [Required]
        public int WrongAnswers { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Score { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal MaxPossibleScore { get; set; }

        public int? TimeTakenSeconds { get; set; }

        public DateTime SubmissionDate { get; set; } = DateTime.Now;

        public bool? IsPassed { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Percentile { get; set; }

        public int? Rank { get; set; }
    }
}
