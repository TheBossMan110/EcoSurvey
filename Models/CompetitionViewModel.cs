using EcoSurvey.Models;

public class CompetitionViewModel
{
    public List<Competition> ActiveCompetitions { get; set; } = new List<Competition>();
    public List<Competition> PastCompetitions { get; set; } = new List<Competition>();
    public List<Winner> TopWinners { get; set; } = new List<Winner>();
}
