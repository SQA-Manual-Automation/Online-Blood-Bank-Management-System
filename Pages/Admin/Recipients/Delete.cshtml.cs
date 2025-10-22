using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BloodBankSystem.Data;
using BloodBankSystem.Models;

namespace BloodBankSystem.Pages_Recipients
{
    public class DeleteModel : PageModel
    {
        private readonly BloodBankSystem.Data.ApplicationDbContext _context;

        public DeleteModel(BloodBankSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Recipient Recipient { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipient = await _context.Recipients.FirstOrDefaultAsync(m => m.Id == id);

            if (recipient is not null)
            {
                Recipient = recipient;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipient = await _context.Recipients.FindAsync(id);
            if (recipient != null)
            {
                Recipient = recipient;
                _context.Recipients.Remove(Recipient);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
