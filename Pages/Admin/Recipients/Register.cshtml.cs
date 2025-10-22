using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BloodBankSystem.Models;
using BloodBankSystem.Data;

namespace BloodBankSystem.Pages_Recipients
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public Recipient Recipient { get; set; } = new Recipient(); // ✅ Initialized

        private readonly BloodBankDbContext _context;

        public RegisterModel(BloodBankDbContext context)
        {
            _context = context;
        }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Recipients.Add(Recipient);
            _context.SaveChanges();

            return RedirectToPage("/Recipients/Dashboard");
        }
    }
}
