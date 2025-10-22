using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloodBankSystem.Data;
using BloodBankSystem.Models;

namespace BloodBankSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/Home/Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("/Recipients")]
        public async Task<IActionResult> Recipients()
        {
            var recipients = await _context.Recipients
                .Include(r => r.BloodRequests)
                .ToListAsync();

            return View("~/Views/Recipient/List.cshtml", recipients);
        }
    }
}
