using System;
using System.ComponentModel.DataAnnotations;

namespace EcoSurvey.Models
{
    public class FAQ
    {
        [Key]
        public int FaqId { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsPublished { get; set; }

        public string Category { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
