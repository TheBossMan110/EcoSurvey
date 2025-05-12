using EcoSurvey.Data;
using EcoSurvey.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EcoSurvey.Controllers
{
    public class SurveyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SurveyController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var surveys = await _context.Surveys
                .Where(s => s.IsActive && s.EndDate >= DateTime.Now)
                .ToListAsync();

            return View(surveys);
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
                SurveyId = surveyId,
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
