using EcoSurvey.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoSurvey.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<CompetitionAnswer> Answers { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<SurveyResult> SurveyResults { get; set; }
        public DbSet<Competition> Competitions { get; set; }
        public DbSet<Winner> Winners { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<SupportInfo> SupportInfo { get; set; }
        public DbSet<Seminar> Seminars { get; set; }
        public DbSet<CompetitionParticipant> CompetitionParticipants { get; set; } // Pluralized
        public DbSet<CompetitionAnswer> CompetitionAnswers { get; set; } // Pluralized
        public DbSet<CompetitionQuestion> CompetitionQuestions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure table names to match SQL schema
            modelBuilder.Entity<Survey>().ToTable("Surveys");
            modelBuilder.Entity<Question>().ToTable("Questions");
            modelBuilder.Entity<CompetitionAnswer>().ToTable("Answers");
            modelBuilder.Entity<Response>().ToTable("Responses");
            modelBuilder.Entity<SurveyResult>().ToTable("SurveyResults");
            modelBuilder.Entity<Competition>().ToTable("Competitions");
            modelBuilder.Entity<Winner>().ToTable("Winners");
            modelBuilder.Entity<FAQ>().ToTable("FAQs");
            modelBuilder.Entity<SupportInfo>().ToTable("SupportInfo");
            modelBuilder.Entity<Seminar>().ToTable("Seminars");
            modelBuilder.Entity<CompetitionParticipant>().ToTable("CompetitionParticipants");
            modelBuilder.Entity<CompetitionAnswer>().ToTable("CompetitionAnswers");
            modelBuilder.Entity<CompetitionQuestion>().ToTable("CompetitionQuestions");

            // Create indexes to match SQL schema
            modelBuilder.Entity<SurveyResult>()
                .HasIndex(sr => sr.SurveyId)
                .HasDatabaseName("IX_SurveyResults_Survey");

            modelBuilder.Entity<SurveyResult>()
                .HasIndex(sr => sr.Score)
                .HasDatabaseName("IX_SurveyResults_Score");

            // Add relationships for new entities
            modelBuilder.Entity<CompetitionParticipant>()
                .HasOne(p => p.Competition)
                .WithMany(c => c.Participants)
                .HasForeignKey(p => p.CompetitionId);

            modelBuilder.Entity<CompetitionAnswer>()
                .HasOne(a => a.Participant)
                .WithMany(p => p.Answers)
                .HasForeignKey(a => a.ParticipantId);

            modelBuilder.Entity<CompetitionAnswer>()
                .HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId);

            modelBuilder.Entity<Winner>()
                .HasOne(w => w.Competition)
                .WithMany(c => c.Winners)
                .HasForeignKey(w => w.CompetitionId);
        }
    }
}
