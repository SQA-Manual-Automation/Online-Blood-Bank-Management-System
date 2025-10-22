using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BloodBankSystem.Data;
using BloodBankSystem.Models;

namespace BloodBankSystem.Pages_Recipients
{
    public class EditModel : PageModel
    {
        private readonly BloodBankSystem.Data.ApplicationDbContext _context;

        public EditModel(BloodBankSystem.Data.ApplicationDbContext context)
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

            var recipient =  await _context.Recipients.FirstOrDefaultAsync(m => m.Id == id);
            if (recipient == null)
            {
                return NotFound();
            }
            Recipient = recipient;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Recipient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipientExists(Recipient.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool RecipientExists(int id)
        {
            return _context.Recipients.Any(e => e.Id == id);
        }
    }
}
