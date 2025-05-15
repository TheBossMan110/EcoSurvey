using System.ComponentModel.DataAnnotations;

namespace EcoSurvey.Models
{
    public class CompetitionSubmissionViewModel
    {
        public int CompetitionId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public int Score { get; set; }

        public List<CompetitionAnswerViewModel> Answers { get; set; } = new List<CompetitionAnswerViewModel>();
    }
}
