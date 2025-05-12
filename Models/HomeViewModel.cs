using System.Collections.Generic;

namespace EcoSurvey.Models
{
    public class HomeViewModel
    {
        public List<Survey> ActiveSurveys { get; set; } = new List<Survey>();
        public List<Competition> ActiveCompetitions { get; set; } = new List<Competition>();
    }
}