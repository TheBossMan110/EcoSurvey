using EcoSurvey.Data;
using EcoSurvey.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
                // Active surveys
                var activeSurveysTask = _context.Surveys
                    .Where(s => s.IsActive && s.EndDate >= DateTime.Now)
                    .OrderByDescending(s => s.StartDate)
                    .Take(5)
                    .ToListAsync();

                // Active competitions
                var activeCompetitionsTask = _context.Competitions
                    .Where(c => c.IsActive && c.EndDate >= DateTime.Now)
                    .OrderByDescending(c => c.StartDate)
                    .Take(5)
                    .ToListAsync();

                // Survey categories with counts
                var categoriesTask = Task.Run(() => {
                    // This is a placeholder - in a real app, you would have a Categories table
                    // and would query the actual categories and counts
                    return new List<CategoryViewModel>
                    {
                        new CategoryViewModel { Name = "Waste Management", Icon = "ri-recycle-line", Count = 12 },
                        new CategoryViewModel { Name = "Energy Conservation", Icon = "ri-water-flash-line", Count = 8 },
                        new CategoryViewModel { Name = "Biodiversity", Icon = "ri-plant-line", Count = 5 },
                        new CategoryViewModel { Name = "Water Conservation", Icon = "ri-drop-line", Count = 7 }
                    };
                });

                // Upcoming events
                var upcomingEventsTask = Task.Run(() => {
                    // This is a placeholder - in a real app, you would have an Events table
                    // and would query the actual events
                    return new List<EventViewModel>
                    {
                        new EventViewModel {
                            Title = "Campus Tree Planting Day",
                            Description = "Join us for our annual tree planting event to increase campus green cover and biodiversity.",
                            Date = new DateTime(2025, 4, 30),
                            Location = "Main Campus Garden",
                            ParticipantCount = 50,
                            ImageUrl = "https://readdy.ai/api/search-image?query=Students%20planting%20trees%20in%20a%20campus%20garden%2C%20environmental%20conservation%20activity%2C%20bright%20natural%20lighting%2C%20educational%20setting%2C%20students%20working%20together%2C%20high%20quality%20realistic%20photography%2C%20environmental%20education&width=600&height=400&seq=eco2&orientation=landscape"
                        },
                        new EventViewModel {
                            Title = "Sustainability Workshop",
                            Description = "Learn practical ways to reduce your carbon footprint and implement sustainable practices.",
                            Date = new DateTime(2025, 5, 5),
                            Location = "Science Building, Room 203",
                            ParticipantCount = 35,
                            ImageUrl = "https://readdy.ai/api/search-image?query=Students%20in%20a%20workshop%20discussing%20sustainability%2C%20environmental%20education%20seminar%2C%20indoor%20classroom%20setting%20with%20presentation%20screens%2C%20diverse%20group%20of%20students%20engaged%20in%20discussion%2C%20bright%20modern%20educational%20space%2C%20realistic%20photography&width=600&height=400&seq=eco3&orientation=landscape"
                        },
                        new EventViewModel {
                            Title = "Community River Cleanup",
                            Description = "Help clean our local river ecosystem and learn about water conservation and pollution.",
                            Date = new DateTime(2025, 5, 12),
                            Location = "Riverside Park",
                            ParticipantCount = 75,
                            ImageUrl = "https://readdy.ai/api/search-image?query=Students%20cleaning%20up%20a%20beach%20or%20riverside%2C%20environmental%20cleanup%20activity%2C%20students%20wearing%20gloves%20and%20collecting%20trash%2C%20bright%20outdoor%20setting%2C%20teamwork%2C%20environmental%20conservation%2C%20realistic%20photography%2C%20high%20quality&width=600&height=400&seq=eco4&orientation=landscape"
                        }
                    };
                });

                // Participation statistics
                var participationStatsTask = Task.Run(() => {
                    // This is a placeholder - in a real app, you would query actual participation data
                    return new ParticipationStatsViewModel
                    {
                        Months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                        SurveyParticipation = new[] { 120, 132, 101, 134, 190, 230 },
                        EventAttendance = new[] { 82, 93, 90, 93, 129, 133 }
                    };
                });

                // Execute all queries in parallel
                await Task.WhenAll(activeSurveysTask, activeCompetitionsTask);

                var viewModel = new HomeViewModel
                {
                    ActiveSurveys = activeSurveysTask.Result,
                    ActiveCompetitions = activeCompetitionsTask.Result,
                    Categories = await categoriesTask,
                    UpcomingEvents = await upcomingEventsTask,
                    ParticipationStats = await participationStatsTask
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
        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
        public List<EventViewModel> UpcomingEvents { get; set; } = new List<EventViewModel>();
        public ParticipationStatsViewModel ParticipationStats { get; set; } = new ParticipationStatsViewModel();
    }

    public class CategoryViewModel
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public int Count { get; set; }
    }

    public class EventViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public int ParticipantCount { get; set; }
        public string ImageUrl { get; set; }
    }

    public class ParticipationStatsViewModel
    {
        public string[] Months { get; set; } = Array.Empty<string>();
        public int[] SurveyParticipation { get; set; } = Array.Empty<int>();
        public int[] EventAttendance { get; set; } = Array.Empty<int>();
    }

    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}