using EcoSurvey.Data;
using EcoSurvey.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EcoSurvey.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var activeSurveysTask = _context.Surveys
                    .Where(s => s.IsActive && s.EndDate >= DateTime.Now)
                    .OrderByDescending(s => s.StartDate)
                    .Take(5)
                    .ToListAsync();

                var activeCompetitionsTask = _context.Competitions
                    .Where(c => c.IsActive && c.EndDate >= DateTime.Now)
                    .OrderByDescending(c => c.StartDate)
                    .Take(5)
                    .ToListAsync();

                // Execute both queries in parallel
                await Task.WhenAll(activeSurveysTask, activeCompetitionsTask);

                var viewModel = new HomeViewModel
                {
                    ActiveSurveys = activeSurveysTask.Result,
                    ActiveCompetitions = activeCompetitionsTask.Result
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page data");
                // Return empty data or redirect to error page
                return View(new HomeViewModel());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class HomeViewModel
    {
        public List<Survey> ActiveSurveys { get; set; } = new List<Survey>();
        public List<Competition> ActiveCompetitions { get; set; } = new List<Competition>();
    }

    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}