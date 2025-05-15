using EcoSurvey.Data;
using EcoSurvey.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EcoSurvey.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SeminarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SeminarController> _logger;

        public SeminarController(
            ApplicationDbContext context,
            ILogger<SeminarController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Admin/Seminar
        public async Task<IActionResult> Index()
        {
            var seminars = await _context.Seminars.ToListAsync();
            return View(seminars);
        }

        // GET: Admin/Seminar/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seminar = await _context.Seminars
                .FirstOrDefaultAsync(m => m.SeminarId == id);

            if (seminar == null)
            {
                return NotFound();
            }

            // Get participants if available
            var participants = await _context.CompetitionParticipants
                .Where(p => p.ParticipantId == id)
                .ToListAsync();

            ViewBag.Participants = participants;
            ViewBag.ParticipantCount = participants.Count;

            return View(seminar);
        }

        // GET: Admin/Seminar/Create
        public IActionResult Create()
        {
            var seminar = new Seminar
            {
                ConductedDate = DateTime.Now.AddDays(7),
                NumberOfParticipants = 20
            };
            return View(seminar);
        }

        // POST: Admin/Seminar/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Seminar seminar)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    seminar.CreatedDate = DateTime.Now;
                    _context.Add(seminar);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Seminar created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating seminar: {@Seminar}", seminar);
                    ModelState.AddModelError("", "An error occurred while saving the seminar. Please try again.");
                }
            }
            return View(seminar);
        }

        // GET: Admin/Seminar/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var seminar = await _context.Seminars.FindAsync(id);
            if (seminar == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/Seminar/Edit.cshtml", seminar);
        }

        // POST: Admin/Seminar/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Seminar seminar)
        {
            if (id != seminar.SeminarId)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
                    _context.Seminars.Update(seminar);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Seminar updated successfully!";
                    var SeminarsList = _context.Seminars.ToList();
                    return View("~/Views/Admin/SeminarManagement.cshtml", SeminarsList);
            //return RedirectToAction(nameof(Index));
            //    }
            //    catch (DbUpdateConcurrencyException ex)
            //    {
            //        if (!SeminarExists(seminar.SeminarId))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            _logger.LogError(ex, "Concurrency error updating seminar: {@Seminar}", seminar);
            //            ModelState.AddModelError("", "The seminar was modified by another user. Please try again.");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex, "Error updating seminar: {@Seminar}", seminar);
            //        ModelState.AddModelError("", "An error occurred while updating the seminar. Please try again.");
            //    }
            //}
        }

        // GET: Admin/Seminar/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seminar = await _context.Seminars
                .FirstOrDefaultAsync(m => m.SeminarId == id);

            if (seminar == null)
            {
                return NotFound();
            }

            return View(seminar);
        }

        // POST: Admin/Seminar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var seminar = await _context.Seminars.FindAsync(id);
                if (seminar == null)
                {
                    return NotFound();
                }

                // Also delete associated participants if any
                var participants = await _context.CompetitionParticipants
                    .Where(p => p.ParticipantId == id)
                    .ToListAsync();

                if (participants.Any())
                {
                    _context.CompetitionParticipants.RemoveRange(participants);
                }

                _context.Seminars.Remove(seminar);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Seminar deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting seminar with ID: {SeminarId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the seminar. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool SeminarExists(int id)
        {
            return _context.Seminars.Any(e => e.SeminarId == id);
        }
    }
}
