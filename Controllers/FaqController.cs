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
    public class FaqController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FaqController> _logger;

        public FaqController(
            ApplicationDbContext context,
            ILogger<FaqController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Admin/Faq
        public async Task<IActionResult> Index()
        {
            var faqs = await _context.FAQs.ToListAsync();
            return View(faqs);
        }

        // GET: Admin/Faq/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faq = await _context.FAQs
                .FirstOrDefaultAsync(m => m.FaqId == id);

            if (faq == null)
            {
                return NotFound();
            }

            return View(faq);
        }

        // GET: Admin/Faq/Create
        public IActionResult Create()
        {
            var faq = new FAQ
            {
                DisplayOrder = _context.FAQs.Count() + 1,
                LastUpdated = DateTime.Now,
                IsPublished = true
            };
            return View(faq);
        }

        // POST: Admin/Faq/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FAQ faq)
        {

                    faq.LastUpdated = DateTime.Now;
                    _context.Add(faq);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "FAQ created successfully!";
                    return RedirectToAction("FaqManagement", "Admin");
        }


        // GET: Admin/Faq/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var faq = await _context.FAQs.FindAsync(id);
            if (faq == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/FAQ/Edit.cshtml", faq);
        }

        // POST: Admin/Faq/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, FAQ faq)
        {
            if (id != faq.FaqId)
            {
                return NotFound();
            }

            _context.FAQs.Update(faq);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "FAQ updated successfully!";
            var FaqsList = _context.FAQs.ToList();
            return View("~/Views/Admin/FaqManagement.cshtml", FaqsList);
        }

        // GET: Admin/Faq/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faq = await _context.FAQs
                .FirstOrDefaultAsync(m => m.FaqId == id);

            if (faq == null)
            {
                return NotFound();
            }

            return View(faq);
        }

        // POST: Admin/Faq/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting FAQ with ID: {FaqId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the FAQ. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool FaqExists(int id)
        {
            return _context.FAQs.Any(e => e.FaqId == id);
        }
    }
}