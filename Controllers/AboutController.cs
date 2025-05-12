using EcoSurvey.Data;
using EcoSurvey.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EcoSurvey.Controllers
{
    public class AboutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AboutController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var faqs = await _context.FAQs.OrderBy(f => f.DisplayOrder).ToListAsync();
            var supportInfo = await _context.SupportInfo.FirstOrDefaultAsync();

            ViewBag.FAQs = faqs;
            ViewBag.SupportInfo = supportInfo;

            return View();
        }
    }
}
