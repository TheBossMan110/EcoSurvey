using EcoSurvey.Data;
using EcoSurvey.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EcoSurvey.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Since we've removed user authentication, we'll just show some dummy data
            // In a real application, you would get the current user's profile

            // Get recent responses
            var responses = await _context.Responses
                .OrderByDescending(r => r.SubmissionDate)
                .Take(5)
                .ToListAsync();

            var surveyIds = responses.Select(r => r.SurveyId).Distinct().ToList();
            var surveys = await _context.Surveys
                .Where(s => surveyIds.Contains(s.SurveyId))
                .ToDictionaryAsync(s => s.SurveyId, s => s);

            ViewBag.Responses = responses;
            ViewBag.Surveys = surveys;

            return View();
        }

        public async Task<IActionResult> Responses()
        {
            // Since we've removed user authentication, we'll just show all responses
            // In a real application, you would filter by the current user
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
