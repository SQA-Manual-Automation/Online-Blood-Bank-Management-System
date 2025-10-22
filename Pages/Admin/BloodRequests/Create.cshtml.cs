using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using BloodBankSystem.Data;
using BloodBankSystem.Models;

namespace BloodBankSystem.Pages.Admin.BloodRequests
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel 
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["RecipientId"] = new SelectList(_context.Recipients, "RecipientId", "RecipientId");
            return Page();
        }

        [BindProperty]
        public BloodRequest BloodRequest { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.BloodRequests.Add(BloodRequest);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
