using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BloodBankSystem.Data;
using BloodBankSystem.Models;

namespace BloodBankSystem.Pages_BloodRequests
{
    public class DetailsModel : PageModel 
    {
        private readonly BloodBankSystem.Data.ApplicationDbContext _context;

        public DetailsModel(BloodBankSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public BloodRequest BloodRequest { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bloodrequest = await _context.BloodRequests.FirstOrDefaultAsync(m => m.Id == id);

            if (bloodrequest is not null)
            {
                BloodRequest = bloodrequest;

                return Page();
            }

            return NotFound();
        }
    }
}
