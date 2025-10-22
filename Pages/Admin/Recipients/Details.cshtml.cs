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
    public class DetailsModel : PageModel
    {
        private readonly BloodBankSystem.Data.ApplicationDbContext _context;

        public DetailsModel(BloodBankSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
