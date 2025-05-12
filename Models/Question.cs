using System;
using System.ComponentModel.DataAnnotations;

namespace EcoSurvey.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        public int SurveyId { get; set; }

        [Required]
        public string QuestionText { get; set; }

        [Required]
        public byte QuestionType { get; set; } // 1=Text, 2=SingleChoice, 3=MultipleChoice, 4=Rating

        public string Options { get; set; } // JSON string for choice options

        public bool IsRequired { get; set; } = true;

        public int DisplayOrder { get; set; }
    }
}
