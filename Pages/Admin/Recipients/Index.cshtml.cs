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
    public class IndexModel : PageModel
    {
        private readonly BloodBankSystem.Data.ApplicationDbContext _context;

        public IndexModel(BloodBankSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Recipient> Recipient { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Recipient = await _context.Recipients.ToListAsync();
        }
    }
}
