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

namespace BloodBankSystem.Pages.BloodRequests
{
    public class EditModel : PageModel  
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BloodRequest? BloodRequest { get; set; }  // ✅ Made nullable to fix CS8600

        public SelectList RecipientOptions { get; set; } = default!;
        public SelectList StatusOptions { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var request = await _context.BloodRequests
                .Include(br => br.Recipient)
                .FirstOrDefaultAsync(br => br.Id == id);

            if (request == null)
                return NotFound();

            BloodRequest = request;

            RecipientOptions = new SelectList(
                _context.Recipients,
                "RecipientId",
                "FullName",
                BloodRequest.RecipientId
            );

            StatusOptions = new SelectList(
                Enum.GetValues(typeof(RequestStatus)).Cast<RequestStatus>().Select(s => new
                {
                    Value = s,
                    Text = s.ToString()
                }),
                "Value",
                "Text",
                BloodRequest.Status
            );

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                RecipientOptions = new SelectList(_context.Recipients, "RecipientId", "FullName");
                StatusOptions = new SelectList(
                    Enum.GetValues(typeof(RequestStatus)).Cast<RequestStatus>().Select(s => new
                    {
                        Value = s,
                        Text = s.ToString()
                    }),
                    "Value",
                    "Text"
                );
                return Page();
            }

            _context.Attach(BloodRequest!).State = EntityState.Modified;  // ✅ Null-forgiving operator

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BloodRequestExists(BloodRequest!.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToPage("./Index");
        }

        private bool BloodRequestExists(int id)
        {
            return _context.BloodRequests.Any(e => e.Id == id);
        }
    }
}
