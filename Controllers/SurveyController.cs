using EcoSurvey.Data;
using EcoSurvey.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Response = EcoSurvey.Models.Response;

namespace EcoSurvey.Controllers
{
    public class SurveyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SurveyController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Admin/Survey
        public async Task<IActionResult> Index()
        {
            var surveys = await _context.Surveys
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync();

            return View(surveys);
        }

        // GET: Admin/Survey/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys
                .FirstOrDefaultAsync(m => m.SurveyId == id);

            if (survey == null)
            {
                return NotFound();
            }

            // Get related questions
            var questions = await _context.Questions
                .Where(q => q.SurveyId == id)
                .OrderBy(q => q.DisplayOrder)
                .ToListAsync();
            ViewBag.Questions = questions;

            // Get response count
            var responseCount = await _context.Responses
                .Where(r => r.SurveyId == id)
                .CountAsync();
            ViewBag.ResponseCount = responseCount;

            // Get associated competitions
            var competitions = await _context.Competitions
                .Where(c => c.SurveyId == id)
                .ToListAsync();
            ViewBag.Competitions = competitions;

            return View("~/Views/Admin/Survey/Details.cshtml", survey);
        }

        // GET: Admin/Survey/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Survey/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Survey survey)
        {
            //if (ModelState.IsValid)
            //{
                survey.CreatedDate = DateTime.Now;
                _context.Add(survey);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Survey created successfully!";
            return RedirectToAction("SurveyManagement", "Admin");
            //}
            //    return View(survey);
        }

        // GET: Admin/Survey/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/Survey/Edit.cshtml", survey);
        }

        // POST: Admin/Survey/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(Survey surveys)
        {
                    _context.Surveys.Update(surveys);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Survey updated successfully!";
                    var SurveysList = _context.Surveys.ToList();
                    return View("~/Views/Admin/SurveyManagement.cshtml", SurveysList);
        }

        // GET: Admin/Survey/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys
                .FirstOrDefaultAsync(m => m.SurveyId == id);

            if (survey == null)
            {
                return NotFound();
            }

            // Check for dependencies
            var questionCount = await _context.Questions
                .Where(q => q.SurveyId == id)
                .CountAsync();
            var responseCount = await _context.Responses
                .Where(r => r.SurveyId == id)
                .CountAsync();
            var competitionCount = await _context.Competitions
                .Where(c => c.SurveyId == id)
                .CountAsync();

            ViewBag.HasDependencies = questionCount > 0 || responseCount > 0 || competitionCount > 0;
            ViewBag.QuestionCount = questionCount;
            ViewBag.ResponseCount = responseCount;
            ViewBag.CompetitionCount = competitionCount;

            return View("~/Views/Admin/Survey/Delete.cshtml", survey);
        }

        // POST: Admin/Survey/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null)
            {
                return NotFound();
            }

            // Delete related questions
            var questions = await _context.Questions
                .Where(q => q.SurveyId == id)
                .ToListAsync();
            _context.Questions.RemoveRange(questions);

            // Delete survey
            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Survey deleted successfully!";
            return RedirectToAction("SurveyManagement", "Admin");
        }

        // Bulk Actions
        [HttpPost]
        public async Task<IActionResult> BulkAction(string action, int[] surveyIds)
        {
            if (surveyIds == null || surveyIds.Length == 0)
            {
                TempData["ErrorMessage"] = "No surveys selected";
                return RedirectToAction(nameof(Index));
            }

            var surveys = await _context.Surveys
                .Where(s => surveyIds.Contains(s.SurveyId))
                .ToListAsync();

            switch (action.ToLower())
            {
                case "activate":
                    surveys.ForEach(s => s.IsActive = true);
                    TempData["SuccessMessage"] = $"{surveys.Count} surveys activated";
                    break;
                case "deactivate":
                    surveys.ForEach(s => s.IsActive = false);
                    TempData["SuccessMessage"] = $"{surveys.Count} surveys deactivated";
                    break;
                case "delete":
                    _context.Surveys.RemoveRange(surveys);
                    TempData["SuccessMessage"] = $"{surveys.Count} surveys deleted";
                    break;
                default:
                    TempData["ErrorMessage"] = "Invalid action";
                    return RedirectToAction(nameof(Index));
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Export
        public IActionResult Export()
        {
            // In a real app, you would generate a CSV or Excel file
            TempData["SuccessMessage"] = "Export feature coming soon!";
            return RedirectToAction(nameof(Index));
        }

        private bool SurveyExists(int id)
        {
            return _context.Surveys.Any(e => e.SurveyId == id);
        }

        public async Task<IActionResult> Quiz(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);

            if (survey == null)
            {
                return NotFound();
            }

            // Check if survey is active
            if (!survey.IsActive || survey.EndDate < DateTime.Now)
            {
                return RedirectToAction("Index", "SurveyBoard");
            }

            // Get questions for this survey
            var questions = await _context.Questions
                .Where(q => q.SurveyId == id)
                .OrderBy(q => q.DisplayOrder)
                .ToListAsync();

            ViewBag.Questions = questions;
            ViewBag.Survey = survey;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitQuiz(int surveyId, [FromForm] IFormCollection formData)
        {
            var survey = await _context.Surveys.FindAsync(surveyId);
            if (survey == null)
            {
                return NotFound();
            }

            // Create a new response
            var response = new Response
            {
                SurveyId = surveyId,
                SubmissionDate = DateTime.Now
            };

            _context.Responses.Add(response);
            await _context.SaveChangesAsync();

            // Get all questions for this survey
            var questions = await _context.Questions
                .Where(q => q.SurveyId == surveyId)
                .ToListAsync();

            int totalQuestions = questions.Count;
            int questionsAttempted = 0;

            // Process each question's answer
            foreach (var question in questions)
            {
                if (formData.TryGetValue($"question_{question.QuestionId}", out var answerValue))
                {
                    questionsAttempted++;

                    // Save the answer
                    var answer = new CompetitionAnswer
                    {
                        QuestionId = question.QuestionId,
                        AnswerId = response.ResponseId,
                        AnswerText = answerValue.ToString()
                    }; 

                    _context.Answers.Add(answer);
                }
                else if (question.QuestionType == 3) // Multiple choice
                {
                    // For multiple choice, check if any option was selected
                    bool hasAnswer = false;
                    var options = !string.IsNullOrEmpty(question.Options)
                        ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(question.Options)
                        : new List<string>();

                    for (int i = 0; i < options.Count; i++)
                    {
                        if (formData.TryGetValue($"question_{question.QuestionId}_{i}", out var optionValue))
                        {
                            hasAnswer = true;

                            // Save each selected option
                            var answer = new CompetitionAnswer
                            {
                                QuestionId = question.QuestionId,
                                AnswerId = response.ResponseId,
                                AnswerText = i.ToString() // Save the option index
                            };

                            _context.Answers.Add(answer);
                        }
                    }

                    if (hasAnswer)
                    {
                        questionsAttempted++;
                    }
                }
            }

            await _context.SaveChangesAsync();

            // Create survey result
            var result = new SurveyResult
            {
                SurveyId = surveyId,
                ResponseId = response.ResponseId,
                TotalQuestions = totalQuestions,
                QuestionsAttempted = questionsAttempted,
                CorrectAnswers = 0, // Not applicable for surveys
                WrongAnswers = 0, // Not applicable for surveys
                Score = (decimal)questionsAttempted / totalQuestions * 100, // Completion percentage
                MaxPossibleScore = 100, // 100%
                SubmissionDate = DateTime.Now
            };

            _context.SurveyResults.Add(result);
            await _context.SaveChangesAsync();

            return RedirectToAction("ThankYou");
        }

        public IActionResult ThankYou()
        {
            return View();
        }

        public async Task<IActionResult> Results(int id)
        {
            var result = await _context.SurveyResults
                .FirstOrDefaultAsync(r => r.ResponseId == id);

            if (result == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys.FindAsync(result.SurveyId);
            ViewBag.Survey = survey;

            return View(result);
        }
    }
}
