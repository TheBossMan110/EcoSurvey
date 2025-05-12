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
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<SurveyResult> SurveyResults { get; set; }
        public DbSet<Competition> Competitions { get; set; }
        public DbSet<Winner> Winners { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<SupportInfo> SupportInfo { get; set; }
        public DbSet<Seminar> Seminars { get; set; }
        public DbSet<EffectiveParticipation> EffectiveParticipation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure table names to match SQL schema
            modelBuilder.Entity<Survey>().ToTable("Surveys");
            modelBuilder.Entity<Question>().ToTable("Questions");
            modelBuilder.Entity<Answer>().ToTable("Answers");
            modelBuilder.Entity<Response>().ToTable("Responses");
            modelBuilder.Entity<SurveyResult>().ToTable("SurveyResults");
            modelBuilder.Entity<Competition>().ToTable("Competitions");
            modelBuilder.Entity<Winner>().ToTable("Winners");
            modelBuilder.Entity<FAQ>().ToTable("FAQs");
            modelBuilder.Entity<SupportInfo>().ToTable("SupportInfo");
            modelBuilder.Entity<Seminar>().ToTable("Seminars");
            modelBuilder.Entity<EffectiveParticipation>().ToTable("EffectiveParticipation");

            // Create indexes to match SQL schema
            modelBuilder.Entity<SurveyResult>()
                .HasIndex(sr => sr.SurveyId)
                .HasDatabaseName("IX_SurveyResults_Survey");

            modelBuilder.Entity<SurveyResult>()
                .HasIndex(sr => sr.Score)
                .HasDatabaseName("IX_SurveyResults_Score");
        }
    }
}
