using System;
using System.ComponentModel.DataAnnotations;

namespace EcoSurvey.Models
{
    public class Answer
    {
        [Key]
        public int AnswerId { get; set; }

        public int QuestionId { get; set; }

        [Required]
        public string AnswerText { get; set; }
    }
}
