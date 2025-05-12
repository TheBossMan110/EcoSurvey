using EcoSurvey.Data;
using EcoSurvey.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EcoSurvey.Controllers
{
    public class SurveyBoardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SurveyBoardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var activeSurveys = await _context.Surveys
                .Where(s => s.IsActive && s.EndDate >= DateTime.Now)
                .ToListAsync();

            var pastSurveys = await _context.Surveys
                .Where(s => !s.IsActive || s.EndDate < DateTime.Now)
                .OrderByDescending(s => s.EndDate)
                .Take(10)
                .ToListAsync();

            ViewBag.ActiveSurveys = activeSurveys;
            ViewBag.PastSurveys = pastSurveys;

            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);

            if (survey == null)
            {
                return NotFound();
            }

            var questions = await _context.Questions
                .Where(q => q.SurveyId == id)
                .OrderBy(q => q.DisplayOrder)
                .ToListAsync();

            ViewBag.Questions = questions;

            return View(survey);
        }

        public async Task<IActionResult> MyResponses()
        {
            // In a real application with authentication, you would filter by user ID
            // For now, we'll just show all responses
            var responses = await _context.Responses
                .OrderByDescending(r => r.SubmissionDate)
                .Take(20)
                .ToListAsync();

            var surveyIds = responses.Select(r => r.SurveyId).Distinct().ToList();
            var surveys = await _context.Surveys
                .Where(s => surveyIds.Contains(s.SurveyId))
                .ToDictionaryAsync(s => s.SurveyId, s => s);

            ViewBag.Surveys = surveys;

            return View(responses);
        }
    }
}
