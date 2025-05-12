using EcoSurvey.Data;
using EcoSurvey.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EcoSurvey.Controllers
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
            try
            {
                var faq = new FAQ
                {
                    DisplayOrder = _context.FAQs.Count() + 1,
                    LastUpdated = DateTime.Now,
                    IsPublished = true
                };
                return View(faq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new FAQ");
                return View(new FAQ { DisplayOrder = 1, LastUpdated = DateTime.Now, IsPublished = true });
            }
        }

        // POST: Admin/Faq/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FAQ faq)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    faq.LastUpdated = DateTime.Now;
                    _context.Add(faq);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "FAQ created successfully!";
                    return RedirectToAction(nameof(Index));
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

        // GET: Admin/Faq/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faq = await _context.FAQs.FindAsync(id);
            if (faq == null)
            {
                return NotFound();
            }
            return View(faq);
        }

        // POST: Admin/Faq/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FAQ faq)
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
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!FaqExists(faq.FaqId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, "Concurrency error updating FAQ: {@FAQ}", faq);
                        ModelState.AddModelError("", "The FAQ was modified by another user. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating FAQ: {@FAQ}", faq);
                    ModelState.AddModelError("", "An error occurred while updating the FAQ. Please try again.");
                }
            }
            return View(faq);
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

        // GET: Admin/Faq/Reorder
        public async Task<IActionResult> Reorder()
        {
            var faqs = await _context.FAQs.OrderBy(f => f.DisplayOrder).ToListAsync();
            return View(faqs);
        }

        // POST: Admin/Faq/UpdateOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrder([FromBody] int[] ids)
        {
            try
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    var faq = await _context.FAQs.FindAsync(ids[i]);
                    if (faq != null)
                    {
                        faq.DisplayOrder = i + 1;
                        _context.Update(faq);
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating FAQ order");
                return Json(new { success = false, message = "An error occurred while updating the order." });
            }
        }

        private bool FaqExists(int id)
        {
            return _context.FAQs.Any(e => e.FaqId == id);
        }
    }
}
