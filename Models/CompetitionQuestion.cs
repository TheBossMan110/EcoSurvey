using System.ComponentModel.DataAnnotations;

namespace EcoSurvey.Models
{
    public class CompetitionQuestion
{
    [Key]
    public int CompetitionQuestionId { get; set; }
    
    public int CompetitionId { get; set; }
    
    public int QuestionId { get; set; }
    
    public int CorrectOptionIndex { get; set; }
    
    // Navigation properties
    public virtual Competition Competition { get; set; }
    public virtual Question Question { get; set; }
}
}