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
            try
            {
                var activeCompetitions = await _context.Competitions
                    .Where(c => c.IsActive && c.EndDate >= DateTime.Now)
                    .OrderByDescending(c => c.StartDate)
                    .ToListAsync();

                var pastCompetitions = await _context.Competitions
                    .Where(c => !c.IsActive || c.EndDate < DateTime.Now)
                    .OrderByDescending(c => c.EndDate)
                    .Take(5)
                    .ToListAsync();

                // Get top winners for display
                var topWinners = await _context.Winners
                    .OrderBy(w => w.Position)
                    .Take(3)
                    .ToListAsync();

                var viewModel = new CompetitionViewModel
                {
                    ActiveCompetitions = activeCompetitions,
                    PastCompetitions = pastCompetitions,
                    TopWinners = topWinners
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the error
                return View(new CompetitionViewModel());
            }
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
        public object Edit(Competition competition)
        {
                   _context.Competitions.Update(competition);
                    _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Competition updated successfully!";

                    var competitionList = _context.Competitions.ToList();
            return View("~/Views/Admin/CompetitionManagement.cshtml", competitionList);
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

        [Route("api/competition/{competitionId}/questions")]
        public IActionResult GetQuestions(int competitionId)
        {
            try
            {
                var competition = _context.Competitions.Find(competitionId);
                if (competition == null || !competition.SurveyId.HasValue)
                {
                    return NotFound();
                }

                var questionsFromDb = _context.Questions
    .Where(q => q.SurveyId == competition.SurveyId &&
                (q.QuestionType == 2 || q.QuestionType == 3))
    .OrderBy(q => q.DisplayOrder)
    .ToList(); // Executes the SQL query and brings data into memory

                var questions = questionsFromDb.Select(q => new
                {
                    questionId = q.QuestionId,
                    question = q.QuestionText,
                    options = !string.IsNullOrEmpty(q.Options) ? q.Options.Split('|') : new string[0],
                    correctAnswer = 0 // still a placeholder
                }).ToList();




                return Json(questions);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, "An error occurred while fetching questions");
            }
        }
        public IActionResult Quiz()
        {
            try
            {

                // Get the active competition
                var activeCompetition = _context.Competitions
                    .Where(c => c.IsActive && c.EndDate >= DateTime.Now)
                    .OrderByDescending(c => c.StartDate)
                    .FirstOrDefault();

                if (activeCompetition == null)
                {
                    // If no active competition, create a default one for display
                    activeCompetition = new Competition
                    {
                        CompetitionId = 0,
                        Title = "Environmental Quiz Competition",
                        Description = "Test your knowledge about environmental issues and sustainability."
                    };
                }

                // Get questions for this competition's survey
                List<Question> questions = new List<Question>();
                if (activeCompetition.SurveyId.HasValue)
                {
                    questions = _context.Questions
                        .Where(q => q.SurveyId == activeCompetition.SurveyId &&
                                   (q.QuestionType == 2 || q.QuestionType == 3)) // Only include choice questions
                        .OrderBy(q => q.DisplayOrder)
                        .ToList();

                    // Log the number of questions found for debugging
                    System.Diagnostics.Debug.WriteLine($"Found {questions.Count} questions for competition {activeCompetition.CompetitionId}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Competition {activeCompetition.CompetitionId} has no SurveyId");
                }

                ViewBag.Competition = activeCompetition;
                ViewBag.Questions = questions;

                // Create a new view model for the form
                var model = new CompetitionSubmissionViewModel
                {
                    CompetitionId = activeCompetition.CompetitionId
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Error in Quiz action: {ex.Message}");
                return View(new CompetitionSubmissionViewModel());
            }
        }

        [HttpPost]
        public IActionResult SubmitQuiz(CompetitionSubmissionViewModel model)
        {
            try
            {
                // Create a new participant
                var participant = new CompetitionParticipant
                {
                    CompetitionId = model.CompetitionId,
                    Name = model.Name,
                    Email = model.Email,
                    Score = model.Score,
                    SubmissionDate = DateTime.Now
                };

                _context.CompetitionParticipants.Add(participant);
                _context.SaveChanges();

                // Redirect to a thank you page or back to the competition page
                TempData["SuccessMessage"] = "Thank you for participating in the quiz!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the error
                TempData["ErrorMessage"] = "An error occurred while submitting your quiz.";
                return RedirectToAction("Quiz");
            }
        }

        public async Task<IActionResult> Results(int id)
        {
            var participant = await _context.CompetitionParticipants
                .Include(p => p.Competition)
                .FirstOrDefaultAsync(p => p.ParticipantId == id);

            if (participant == null)
            {
                return NotFound();
            }

            // Get top participants for leaderboard
            var leaderboard = await _context.CompetitionParticipants
                .Where(p => p.CompetitionId == participant.CompetitionId)
                .OrderByDescending(p => p.Score)
                .Take(10)
                .ToListAsync();

            ViewBag.Leaderboard = leaderboard;
            ViewBag.Rank = leaderboard.FindIndex(p => p.ParticipantId == id) + 1;

            return View(participant);
        }
        public IActionResult ThankYou()
        {
            return View();
        }
    }
}
