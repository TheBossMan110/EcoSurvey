﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoSurvey.Models
{
    public class Winner
    {
        [Key]
        public int WinnerId { get; set; }

        public int CompetitionId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        public int Position { get; set; }

        public int Score { get; set; }

        public DateTime AwardDate { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("CompetitionId")]
        public virtual Competition Competition { get; set; }
    }
}