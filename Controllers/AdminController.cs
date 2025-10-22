using Microsoft.AspNetCore.Mvc;
using BloodBankSystem.Data;
using BloodBankSystem.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace BloodBankSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Admin admin)
        {
            if (ModelState.IsValid)
            {
                var matchedAdmin = _context.Admins
                    .FirstOrDefault(a => a.Username == admin.Username && a.Password == admin.Password);

                if (matchedAdmin != null)
                {
                    HttpContext.Session.SetString("AdminLoggedIn", "true");
                    return RedirectToAction("Index", "Admin"); 
                }

                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }

            return View(admin);
        }


        public IActionResult Index()
        {

            ViewBag.Message = "Welcome, Admin. This is your dashboard.";
            return View();
        }

   
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminLoggedIn");
            return RedirectToAction("Login");
        }
    }
}
