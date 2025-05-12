using EcoSurvey.Data;
using EcoSurvey.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EcoSurvey.Controllers
{
    public class CompetitionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompetitionController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Admin/Competition
        public async Task<IActionResult> Index()
        {
            var competitions = await _context.Competitions
                .Include(c => c.Survey)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();

            // Get survey titles for display
            var surveyIds = competitions.Where(c => c.SurveyId.HasValue).Select(c => c.SurveyId.Value).Distinct();
            var surveys = await _context.Surveys
                .Where(s => surveyIds.Contains(s.SurveyId))
                .ToDictionaryAsync(s => s.SurveyId, s => s.Title);

            ViewBag.SurveyTitles = surveys;
            return View(competitions);
        }

        // GET: Admin/Competition/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions
                .Include(c => c.Survey)
                .FirstOrDefaultAsync(m => m.CompetitionId == id);

            if (competition == null)
            {
                return NotFound();
            }

            return View(competition);
        }

        // GET: Admin/Competition/Create
        public async Task<IActionResult> Create()
        {
            var surveys = await _context.Surveys
                .Where(s => s.IsActive && s.EndDate >= DateTime.Now)
                .Select(s => new { s.SurveyId, s.Title })
                .ToListAsync();

            ViewBag.Surveys = new SelectList(surveys, "SurveyId", "Title");
            return View();
        }

        // POST: Admin/Competition/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Competition competition)
        {
            //if (ModelState.IsValid)
            //{
                competition.CreatedDate = DateTime.Now;
                _context.Add(competition);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Competition created successfully!";
            //return RedirectToAction("Index")
                return RedirectToAction("CompetitionManagement", "Admin");

            //}

            //var surveys = await _context.Surveys
            //    .Where(s => s.IsActive && s.EndDate >= DateTime.Now)
            //    .Select(s => new { s.SurveyId, s.Title })
            //    .ToListAsync();

            //ViewBag.Surveys = new SelectList(surveys, "SurveyId", "Title", competition.SurveyId);
            //return View(competition);
        }

        // GET: Admin/Competition/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions.FindAsync(id);
            if (competition == null)
            {
                return NotFound();
            }

            var surveys = await _context.Surveys
                .Where(s => s.IsActive && s.EndDate >= DateTime.Now)
                .Select(s => new { s.SurveyId, s.Title })
                .ToListAsync();

            ViewBag.Surveys = new SelectList(surveys, "SurveyId", "Title", competition.SurveyId);
            return View("~/Views/Admin/Competition/Edit.cshtml",competition);
        }

        // POST: Admin/Competition/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompetitionId,SurveyId,Title,Description,StartDate,EndDate,IsActive,CreatedDate,Rules,Prizes,MaxParticipants")] Competition competition)
        {
            if (id != competition.CompetitionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(competition);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Competition updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompetitionExists(competition.CompetitionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            var surveys = await _context.Surveys
                .Where(s => s.IsActive && s.EndDate >= DateTime.Now)
                .Select(s => new { s.SurveyId, s.Title })
                .ToListAsync();

            ViewBag.Surveys = new SelectList(surveys, "SurveyId", "Title", competition.SurveyId);
            return View(competition);
        }

        // GET: Admin/Competition/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions
                .Include(c => c.Survey)
                .FirstOrDefaultAsync(m => m.CompetitionId == id);

            if (competition == null)
            {
                return NotFound();
            }

            return View(competition);
        }

        // POST: Admin/Competition/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var competition = await _context.Competitions.FindAsync(id);
            _context.Competitions.Remove(competition);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Competition deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool CompetitionExists(int id)
        {
            return _context.Competitions.Any(e => e.CompetitionId == id);
        }


        //public async Task<IActionResult> Index()
        //{
        //    var activeCompetitions = await _context.Competitions
        //        .Where(c => c.IsActive && c.EndDate >= DateTime.Now)
        //        .ToListAsync();

        //    var pastCompetitions = await _context.Competitions
        //        .Where(c => !c.IsActive || c.EndDate < DateTime.Now)
        //        .OrderByDescending(c => c.EndDate)
        //        .Take(10)
        //        .ToListAsync();

        //    ViewBag.ActiveCompetitions = activeCompetitions;
        //    ViewBag.PastCompetitions = pastCompetitions;

        //    return View();
        //}

        public async Task<IActionResult> Details(int id)
        {
            var competition = await _context.Competitions.FindAsync(id);

            if (competition == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys.FindAsync(competition.SurveyId);
            ViewBag.Survey = survey;

            var winners = await _context.Winners
                .Where(w => w.CompetitionId == id)
                .OrderBy(w => w.Position)
                .ToListAsync();
            ViewBag.Winners = winners;

            return View(competition);
        }

        public async Task<IActionResult> Quiz(int id)
        {
            var competition = await _context.Competitions.FindAsync(id);

            if (competition == null)
            {
                return NotFound();
            }

            // Check if competition is active
            if (!competition.IsActive || competition.EndDate < DateTime.Now)
            {
                return RedirectToAction("Index");
            }

            var survey = await _context.Surveys.FindAsync(competition.SurveyId);
            if (survey == null)
            {
                return NotFound();
            }

            // Get questions for this survey
            var questions = await _context.Questions
                .Where(q => q.SurveyId == survey.SurveyId)
                .OrderBy(q => q.DisplayOrder)
                .ToListAsync();

            ViewBag.Questions = questions;
            ViewBag.Survey = survey;
            ViewBag.Competition = competition;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitQuiz(int competitionId, [FromForm] IFormCollection formData)
        {
            var competition = await _context.Competitions.FindAsync(competitionId);
            if (competition == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys.FindAsync(competition.SurveyId);
            if (survey == null)
            {
                return NotFound();
            }

            // Create a new response
            var response = new Response
            {
                SurveyId = survey.SurveyId,
                SubmissionDate = DateTime.Now
            };

            _context.Responses.Add(response);
            await _context.SaveChangesAsync();

            // Get all questions for this survey
            var questions = await _context.Questions
                .Where(q => q.SurveyId == survey.SurveyId)
                .ToListAsync();

            int totalQuestions = questions.Count;
            int questionsAttempted = 0;
            int correctAnswers = 0;
            int wrongAnswers = 0;

            // Process each question's answer
            foreach (var question in questions)
            {
                if (formData.TryGetValue($"question_{question.QuestionId}", out var answerValue))
                {
                    questionsAttempted++;

                    // Save the answer
                    var answer = new Answer
                    {
                        QuestionId = question.QuestionId,
                        AnswerText = answerValue.ToString()
                    };

                    _context.Answers.Add(answer);

                    // For simplicity, we're not calculating correct/wrong answers here
                    // In a real application, you would compare with correct answers
                }
            }

            await _context.SaveChangesAsync();

            // Create survey result
            var result = new SurveyResult
            {
                SurveyId = survey.SurveyId,
                ResponseId = response.ResponseId,
                TotalQuestions = totalQuestions,
                QuestionsAttempted = questionsAttempted,
                CorrectAnswers = correctAnswers,
                WrongAnswers = wrongAnswers,
                Score = 0, // Calculate based on your scoring system
                MaxPossibleScore = totalQuestions, // Assuming 1 point per question
                SubmissionDate = DateTime.Now
            };

            _context.SurveyResults.Add(result);
            await _context.SaveChangesAsync();

            // Collect participant information for competition
            string participantName = formData["ParticipantName"].ToString();
            string participantEmail = formData["ParticipantEmail"].ToString();

            // Store this information for potential winners
            TempData["ParticipantName"] = participantName;
            TempData["ParticipantEmail"] = participantEmail;
            TempData["CompetitionId"] = competitionId;
            TempData["Score"] = result.Score;

            return RedirectToAction("ThankYou");
        }

        public IActionResult ThankYou()
        {
            return View();
        }
    }
}
