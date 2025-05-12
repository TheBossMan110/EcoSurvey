using System;
using System.ComponentModel.DataAnnotations;

namespace EcoSurvey.Models
{
    public class SupportInfo
    {
        [Key]
        public int InfoId { get; set; }

        [Required]
        public string ContactDetails { get; set; }

        public string TechnicalHelp { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
