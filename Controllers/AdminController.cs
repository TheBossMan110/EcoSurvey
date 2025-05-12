using EcoSurvey.Data;
using EcoSurvey.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace EcoSurvey.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            ApplicationDbContext context,
            ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardViewModel = new
            {
                TotalSurveys = await _context.Surveys.CountAsync(),
                ActiveSurveys = await _context.Surveys.CountAsync(s => s.IsActive && s.EndDate >= DateTime.Now),
                TotalCompetitions = await _context.Competitions.CountAsync(),
                TotalResponses = await _context.Responses.CountAsync()
            };

            return View(dashboardViewModel);
        }

        #region User Management

        public IActionResult UserManagement()
        {
            // This is a placeholder for now - in a real application, you would fetch users from your identity system
            var users = new List<object>
            {
                new { Id = 1, Name = "John Doe", Email = "john.doe@example.com", Role = "Admin", Status = "Active", RegisteredDate = DateTime.Now.AddDays(-30), LastLogin = DateTime.Now.AddDays(-1) },
                new { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com", Role = "User", Status = "Active", RegisteredDate = DateTime.Now.AddDays(-25), LastLogin = DateTime.Now.AddDays(-2) },
                new { Id = 3, Name = "Bob Johnson", Email = "bob.johnson@example.com", Role = "Moderator", Status = "Inactive", RegisteredDate = DateTime.Now.AddDays(-60), LastLogin = DateTime.Now.AddDays(-30) },
                new { Id = 4, Name = "Alice Brown", Email = "alice.brown@example.com", Role = "User", Status = "Pending", RegisteredDate = DateTime.Now.AddDays(-5), LastLogin = (DateTime?)null },
                new { Id = 5, Name = "Charlie Davis", Email = "charlie.davis@example.com", Role = "User", Status = "Blocked", RegisteredDate = DateTime.Now.AddDays(-90), LastLogin = DateTime.Now.AddDays(-45) }
            };

            return View(users);
        }

        #endregion

        #region Settings

        public IActionResult Settings()
        {
            // This is a placeholder for system settings
            var settings = new
            {
                SiteName = "Environmental Awareness Survey Portal",
                SiteDescription = "A platform for environmental awareness surveys, competitions, and seminars.",
                ContactEmail = "contact@ecosurvey.example.com",
                SupportEmail = "support@ecosurvey.example.com",
                FooterText = "© 2023 Environmental Awareness Survey Portal. All rights reserved.",
                MaintenanceMode = false,
                PrimaryColor = "#10B981",
                SecondaryColor = "#3B82F6",
                ThemeMode = "light",
                AllowRegistration = true,
                RequireEmailVerification = true,
                AllowSocialLogin = true,
                DefaultUserRole = "User",
                AccountApproval = "Automatic",
                SessionTimeout = 30,
                MaxLoginAttempts = 5,
                PasswordPolicy = "Standard"
            };

            return View(settings);
        }

        #endregion

        #region Survey Management

        public async Task<IActionResult> SurveyManagement()
        {
            var surveys = await _context.Surveys.ToListAsync();
            return View(surveys);
        }

        public IActionResult CreateSurvey()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSurvey(Survey survey)
        {
            if (ModelState.IsValid)
            {
                survey.CreatedDate = DateTime.Now;
                _context.Surveys.Add(survey);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SurveyManagement));
            }
            return View(survey);
        }

        public async Task<IActionResult> EditSurvey(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSurvey(int id, Survey survey)
        {
            if (id != survey.SurveyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(survey);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurveyExists(survey.SurveyId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(SurveyManagement));
            }
            return View(survey);
        }

        public async Task<IActionResult> DeleteSurvey(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null)
            {
                return NotFound();
            }

            return View(survey);
        }

        [HttpPost, ActionName("DeleteSurvey")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSurveyConfirmed(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);
            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(SurveyManagement));
        }

        private bool SurveyExists(int id)
        {
            return _context.Surveys.Any(e => e.SurveyId == id);
        }

        #endregion

        #region Competition Management

        public async Task<IActionResult> CompetitionManagement()
        {
            var competitions = await _context.Competitions.ToListAsync();

            // Get associated survey titles
            var surveyIds = competitions.Select(c => c.SurveyId).Distinct().ToList();
            var surveys = await _context.Surveys
                .Where(s => surveyIds.Contains(s.SurveyId))
                .ToDictionaryAsync(s => s.SurveyId, s => s.Title);

            ViewBag.SurveyTitles = surveys;

            return View(competitions);
        }

        public async Task<IActionResult> CreateCompetition()
        {
            // Get all surveys for dropdown
            var surveys = await _context.Surveys
                .Where(s => s.IsActive)
                .ToListAsync();

            ViewBag.Surveys = surveys;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCompetition(Competition competition)
        {
            // Log the received model to help with debugging
            _logger.LogInformation("Received competition model: {@Competition}", competition);

            // Manually validate the model to see what's failing
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid: {@ModelStateErrors}",
                    ModelState.Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(k => k.Key, v => v.Value.Errors.Select(e => e.ErrorMessage).ToList()));

                // Get all surveys for dropdown
                var surveys = await _context.Surveys
                    .Where(s => s.IsActive)
                    .ToListAsync();

                ViewBag.Surveys = surveys;

                // Return the view with the model to show validation errors
                return View(competition);
            }

            try
            {
                // Ensure required fields are set
                competition.CreatedDate = DateTime.Now;
                competition.IsActive = true;

                // Add to database and save changes
                _context.Competitions.Add(competition);
                await _context.SaveChangesAsync();

                // Log success
                _logger.LogInformation("Competition created successfully: {@Competition}", competition);

                // Redirect to the competition management page
                TempData["SuccessMessage"] = "Competition created successfully!";
                return RedirectToAction(nameof(CompetitionManagement));
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error creating competition: {@Competition}", competition);

                // Add error to model state
                ModelState.AddModelError("", "An error occurred while saving the competition. Please try again.");

                // Get all surveys for dropdown
                var surveys = await _context.Surveys
                    .Where(s => s.IsActive)
                    .ToListAsync();

                ViewBag.Surveys = surveys;

                // Return the view with the model
                return View(competition);
            }
        }

        public async Task<IActionResult> EditCompetition(int id)
        {
            var competition = await _context.Competitions.FindAsync(id);
            if (competition == null)
            {
                return NotFound();
            }

            // Get all surveys for dropdown
            var surveys = await _context.Surveys.ToListAsync();
            ViewBag.Surveys = surveys;

            return View(competition);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCompetition(int id, Competition competition)
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
                return RedirectToAction(nameof(CompetitionManagement));
            }

            // If we got this far, something failed, redisplay form
            var surveys = await _context.Surveys.ToListAsync();
            ViewBag.Surveys = surveys;
            return View(competition);
        }

        public async Task<IActionResult> DeleteCompetition(int id)
        {
            var competition = await _context.Competitions.FindAsync(id);
            if (competition == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys.FindAsync(competition.SurveyId);
            ViewBag.Survey = survey;

            return View(competition);
        }

        [HttpPost, ActionName("DeleteCompetition")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCompetitionConfirmed(int id)
        {
            var competition = await _context.Competitions.FindAsync(id);
            _context.Competitions.Remove(competition);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Competition deleted successfully!";
            return RedirectToAction(nameof(CompetitionManagement));
        }

        private bool CompetitionExists(int id)
        {
            return _context.Competitions.Any(e => e.CompetitionId == id);
        }

        #endregion

        #region Seminar Management

        public async Task<IActionResult> SeminarManagement()
        {
            var seminars = await _context.Seminars.ToListAsync();
            return View(seminars);
        }

        public IActionResult CreateSeminar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSeminar(Seminar seminar)
        {
            // Log the received model to help with debugging
            _logger.LogInformation("Received seminar model: {@Seminar}", seminar);

            // Manually validate the model to see what's failing
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid: {@ModelStateErrors}",
                    ModelState.Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(k => k.Key, v => v.Value.Errors.Select(e => e.ErrorMessage).ToList()));

                // Return the view with the model to show validation errors
                return View(seminar);
            }

            try
            {
                // Ensure required fields are set
                seminar.CreatedDate = DateTime.Now;

                // Add to database and save changes
                _context.Seminars.Add(seminar);
                await _context.SaveChangesAsync();

                // Log success
                _logger.LogInformation("Seminar created successfully: {@Seminar}", seminar);

                // Redirect to the seminar management page
                TempData["SuccessMessage"] = "Seminar created successfully!";
                return RedirectToAction(nameof(SeminarManagement));
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error creating seminar: {@Seminar}", seminar);

                // Add error to model state
                ModelState.AddModelError("", "An error occurred while saving the seminar. Please try again.");

                // Return the view with the model
                return View(seminar);
            }
        }

        public async Task<IActionResult> EditSeminar(int id)
        {
            var seminar = await _context.Seminars.FindAsync(id);
            if (seminar == null)
            {
                return NotFound();
            }
            return View(seminar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSeminar(int id, Seminar seminar)
        {
            if (id != seminar.SeminarId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seminar);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Seminar updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeminarExists(seminar.SeminarId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(SeminarManagement));
            }
            return View(seminar);
        }

        public async Task<IActionResult> DeleteSeminar(int id)
        {
            var seminar = await _context.Seminars.FindAsync(id);
            if (seminar == null)
            {
                return NotFound();
            }

            return View(seminar);
        }

        [HttpPost, ActionName("DeleteSeminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSeminarConfirmed(int id)
        {
            var seminar = await _context.Seminars.FindAsync(id);
            _context.Seminars.Remove(seminar);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Seminar deleted successfully!";
            return RedirectToAction(nameof(SeminarManagement));
        }

        private bool SeminarExists(int id)
        {
            return _context.Seminars.Any(e => e.SeminarId == id);
        }

        #endregion

        #region Analytics

        public async Task<IActionResult> Analytics()
        {
            // Get basic statistics
            var totalSurveys = await _context.Surveys.CountAsync();
            var totalResponses = await _context.Responses.CountAsync();
            var totalCompetitions = await _context.Competitions.CountAsync();
            var totalSeminars = await _context.Seminars.CountAsync();

            // Calculate completion rates
            var completionRates = new List<object>();
            var surveys = await _context.Surveys.ToListAsync();
            foreach (var survey in surveys)
            {
                var responses = await _context.Responses.CountAsync(r => r.SurveyId == survey.SurveyId);
                var results = await _context.SurveyResults.CountAsync(sr => sr.SurveyId == survey.SurveyId);

                var completionRate = responses > 0 ? (double)results / responses * 100 : 0;

                completionRates.Add(new
                {
                    SurveyId = survey.SurveyId,
                    Title = survey.Title,
                    Responses = responses,
                    CompletionRate = Math.Round(completionRate, 1)
                });
            }

            // Get user distribution by type
            var userDistribution = await _context.Surveys
                .GroupBy(s => s.TargetUserType)
                .Select(g => new { UserType = g.Key, Count = g.Count() })
                .ToListAsync();

            // Get device usage (placeholder since we don't have this data in the model)
            var deviceUsage = new[]
            {
                new { DeviceType = "Mobile", Percentage = 58 },
                new { DeviceType = "Desktop", Percentage = 32 },
                new { DeviceType = "Tablet", Percentage = 10 }
            };

            // Environmental impact metrics (placeholder)
            var environmentalImpact = new
            {
                WasteRecycled = 2450,
                WaterSaved = 12800,
                EnergyConserved = 8750,
                CO2Reduced = 42.5
            };

            // Pass all data to the view
            ViewBag.TotalSurveys = totalSurveys;
            ViewBag.TotalResponses = totalResponses;
            ViewBag.TotalCompetitions = totalCompetitions;
            ViewBag.TotalSeminars = totalSeminars;
            ViewBag.CompletionRates = completionRates;
            ViewBag.UserDistribution = userDistribution;
            ViewBag.DeviceUsage = deviceUsage;
            ViewBag.EnvironmentalImpact = environmentalImpact;

            return View();
        }

        #endregion

        #region Reports

        public async Task<IActionResult> Reports()
        {
            // Get surveys for report generation
            var surveys = await _context.Surveys.ToListAsync();
            ViewBag.Surveys = surveys;

            // Get competitions for report generation
            var competitions = await _context.Competitions.ToListAsync();
            ViewBag.Competitions = competitions;

            // Get seminars for report generation
            var seminars = await _context.Seminars.ToListAsync();
            ViewBag.Seminars = seminars;

            return View();
        }

        #endregion

        #region FAQ Management

        public async Task<IActionResult> FaqManagement()
        {
            try
            {
                var faqs = await _context.FAQs.OrderBy(f => f.DisplayOrder).ToListAsync();
                return View(faqs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving FAQs");
                return View(new List<FAQ>());
            }
        }

        public IActionResult CreateFaq()
        {
            try
            {
                var faq = new FAQ
                {
                    DisplayOrder = _context.FAQs.Count() + 1,
                    LastUpdated = DateTime.Now
                };
                return View(faq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new FAQ");
                return View(new FAQ { DisplayOrder = 1, LastUpdated = DateTime.Now });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFaq(FAQ faq)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    faq.LastUpdated = DateTime.Now;
                    _context.FAQs.Add(faq);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "FAQ created successfully!";
                    return RedirectToAction(nameof(FaqManagement));
                }
                return View(faq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving FAQ: {@FAQ}", faq);
                ModelState.AddModelError("", "An error occurred while saving the FAQ. Please try again.");
                return View(faq);
            }
        }

        public async Task<IActionResult> EditFaq(int id)
        {
            try
            {
                var faq = await _context.FAQs.FindAsync(id);
                if (faq == null)
                {
                    return NotFound();
                }
                return View(faq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving FAQ with ID: {FaqId}", id);
                return RedirectToAction(nameof(FaqManagement));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFaq(int id, FAQ faq)
        {
            try
            {
                if (id != faq.FaqId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        faq.LastUpdated = DateTime.Now;
                        _context.Update(faq);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "FAQ updated successfully!";
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!FaqExists(faq.FaqId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(FaqManagement));
                }
                return View(faq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating FAQ: {@FAQ}", faq);
                ModelState.AddModelError("", "An error occurred while updating the FAQ. Please try again.");
                return View(faq);
            }
        }

        public async Task<IActionResult> DeleteFaq(int id)
        {
            try
            {
                var faq = await _context.FAQs.FindAsync(id);
                if (faq == null)
                {
                    return NotFound();
                }

                return View(faq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving FAQ for deletion with ID: {FaqId}", id);
                return RedirectToAction(nameof(FaqManagement));
            }
        }

        [HttpPost, ActionName("DeleteFaq")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFaqConfirmed(int id)
        {
            try
            {
                var faq = await _context.FAQs.FindAsync(id);
                if (faq == null)
                {
                    return NotFound();
                }

                _context.FAQs.Remove(faq);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "FAQ deleted successfully!";
                return RedirectToAction(nameof(FaqManagement));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting FAQ with ID: {FaqId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the FAQ. Please try again.";
                return RedirectToAction(nameof(FaqManagement));
            }
        }

        private bool FaqExists(int id)
        {
            return _context.FAQs.Any(e => e.FaqId == id);
        }

        #endregion
    }
}
