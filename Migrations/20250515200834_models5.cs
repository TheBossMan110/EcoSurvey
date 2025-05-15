using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoSurvey.Migrations
{
    /// <inheritdoc />
    public partial class models5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "EffectiveParticipation");

            migrationBuilder.DropColumn(
                name: "ParticipantEmail",
                table: "Winners");

            migrationBuilder.RenameColumn(
                name: "Prize",
                table: "Winners",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ParticipantName",
                table: "Winners",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "AwardedDate",
                table: "Winners",
                newName: "AwardDate");

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Winners",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CompetitionParticipants",
                columns: table => new
                {
                    ParticipantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompetitionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitionParticipants", x => x.ParticipantId);
                    table.ForeignKey(
                        name: "FK_CompetitionParticipants_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "CompetitionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompetitionAnswers",
                columns: table => new
                {
                    AnswerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitionAnswers", x => x.AnswerId);
                    table.ForeignKey(
                        name: "FK_CompetitionAnswers_CompetitionParticipants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "CompetitionParticipants",
                        principalColumn: "ParticipantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompetitionAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Winners_CompetitionId",
                table: "Winners",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionAnswers_ParticipantId",
                table: "CompetitionAnswers",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionAnswers_QuestionId",
                table: "CompetitionAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionParticipants_CompetitionId",
                table: "CompetitionParticipants",
                column: "CompetitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Winners_Competitions_CompetitionId",
                table: "Winners",
                column: "CompetitionId",
                principalTable: "Competitions",
                principalColumn: "CompetitionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Winners_Competitions_CompetitionId",
                table: "Winners");

            migrationBuilder.DropTable(
                name: "CompetitionAnswers");

            migrationBuilder.DropTable(
                name: "CompetitionParticipants");

            migrationBuilder.DropIndex(
                name: "IX_Winners_CompetitionId",
                table: "Winners");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Winners");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Winners",
                newName: "Prize");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Winners",
                newName: "ParticipantName");

            migrationBuilder.RenameColumn(
                name: "AwardDate",
                table: "Winners",
                newName: "AwardedDate");

            migrationBuilder.AddColumn<string>(
                name: "ParticipantEmail",
                table: "Winners",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    AnswerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    ResponseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.AnswerId);
                });

            migrationBuilder.CreateTable(
                name: "EffectiveParticipation",
                columns: table => new
                {
                    ParticipationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParticipantName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParticipationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SeminarId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EffectiveParticipation", x => x.ParticipationId);
                });
        }
    }
}
