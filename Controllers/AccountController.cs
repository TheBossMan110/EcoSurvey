using EcoSurvey.Data;
using EcoSurvey.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EcoSurvey.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // Since we've removed user authentication, we'll just redirect to home
            // In a real application, you would validate credentials and sign in the user
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View("Login");
        }

        [HttpPost]
        public IActionResult Register(string email, string password, string confirmPassword)
        {
            // Since we've removed user authentication, we'll just redirect to home
            // In a real application, you would create a new user account
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            // Since we've removed user authentication, we'll just redirect to home
            // In a real application, you would sign out the user
            return RedirectToAction("Index", "Home");
        }
    }
}
