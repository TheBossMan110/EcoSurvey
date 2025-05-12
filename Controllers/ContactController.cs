using EcoSurvey.Data;
using EcoSurvey.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace EcoSurvey.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var supportInfo = await _context.SupportInfo.FirstOrDefaultAsync();
            return View(supportInfo);
        }

        [HttpPost]
        public IActionResult SendMessage(string name, string email, string subject, string message)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message))
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return RedirectToAction("Index");
            }

            // In a real application, you would send the email here
            // For now, we'll just show a success message
            TempData["SuccessMessage"] = "Your message has been sent successfully.";
            return RedirectToAction("Index");
        }
    }
}
