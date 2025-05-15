using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoSurvey.Models
{
    public class CompetitionAnswer
    {
        [Key]
        public int AnswerId { get; set; }

        public int ParticipantId { get; set; }

        public int QuestionId { get; set; }

        [Required]
        public string AnswerText { get; set; }

        public bool IsCorrect { get; set; }

        // Navigation properties
        [ForeignKey("ParticipantId")]
        public virtual CompetitionParticipant Participant { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
    }
}