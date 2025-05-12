using System;
using System.ComponentModel.DataAnnotations;

namespace EcoSurvey.Models
{
    public class Response
    {
        [Key]
        public int ResponseId { get; set; }

        public int SurveyId { get; set; }

        public DateTime SubmissionDate { get; set; } = DateTime.Now;
    }
}
