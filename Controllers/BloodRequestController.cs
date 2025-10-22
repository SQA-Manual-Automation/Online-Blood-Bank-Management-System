using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloodBankSystem.Data;
using BloodBankSystem.Models;

namespace BloodBankSystem.Controllers
{
    public class BloodRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BloodRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Admin access: show all requests
            if (HttpContext.Session.GetString("AdminLoggedIn") == "true")
            {
                var allRequests = await _context.BloodRequests
                    .Include(b => b.Recipient)
                    .AsNoTracking()
                    .ToListAsync();

                return View(allRequests);
            }

            // Recipient access: show only their requests
            var userEmail = User.Identity?.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["ToastMessage"] = "User email not found.";
                return RedirectToAction("Index", "Home");
            }

            var recipient = await _context.Recipients
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Email == userEmail);

            if (recipient == null)
            {
                TempData["ToastMessage"] = "Recipient profile not found.";
                return RedirectToAction("Index", "Home");
            }

            var requests = await _context.BloodRequests
                .Include(b => b.Recipient)
                .Where(b => b.RecipientId == recipient.Id)
                .ToListAsync();

            return View(requests);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var bloodRequest = await _context.BloodRequests
                .Include(b => b.Recipient)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bloodRequest == null) return NotFound();

            return View(bloodRequest);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BloodGroup,Quantity,Location,Message,RequestDate,RequestedAt,Status")] BloodRequest bloodRequest)
        {
            var userEmail = User.Identity?.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["ToastMessage"] = "User email not found.";
                return RedirectToAction("Index", "Home");
            }

            var recipient = await _context.Recipients.FirstOrDefaultAsync(r => r.Email == userEmail);

            if (recipient == null)
            {
                TempData["ToastMessage"] = "Recipient profile not found.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                bloodRequest.RecipientId = recipient.Id;
                bloodRequest.RequestedAt = DateTime.Now;

                _context.Add(bloodRequest);
                await _context.SaveChangesAsync();

                TempData["ToastMessage"] = "Blood request submitted successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(bloodRequest);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var bloodRequest = await _context.BloodRequests.FindAsync(id);

            if (bloodRequest == null) return NotFound();

            return View(bloodRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BloodGroup,Quantity,Location,Message,RequestDate,RequestedAt,Status")] BloodRequest bloodRequest)
        {
            if (id != bloodRequest.Id) return NotFound();

            var userEmail = User.Identity?.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["ToastMessage"] = "User email not found.";
                return RedirectToAction("Index", "Home");
            }

            var recipient = await _context.Recipients.FirstOrDefaultAsync(r => r.Email == userEmail);

            if (recipient == null)
            {
                TempData["ToastMessage"] = "Recipient profile not found.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var existingRequest = await _context.BloodRequests.FindAsync(id);
                if (existingRequest == null) return NotFound();

                try
                {
                    bloodRequest.RecipientId = recipient.Id;
                    _context.Entry(existingRequest).CurrentValues.SetValues(bloodRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BloodRequestExists(bloodRequest.Id)) return NotFound();
                    else throw;
                }

                TempData["ToastMessage"] = "Blood request updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(bloodRequest);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bloodRequest = await _context.BloodRequests
                .Include(b => b.Recipient)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bloodRequest == null) return NotFound();

            return View(bloodRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bloodRequest = await _context.BloodRequests.FindAsync(id);

            if (bloodRequest != null)
            {
                _context.BloodRequests.Remove(bloodRequest);
                await _context.SaveChangesAsync();
                TempData["ToastMessage"] = "Blood request deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BloodRequestExists(int id)
        {
            return _context.BloodRequests.Any(e => e.Id == id);
        }
    }
}
