using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BloodBankSystem.Data;
using BloodBankSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloodBankSystem.Controllers
{
    public class BloodRequestAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BloodRequestAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var requests = await _context.BloodRequests
                .Include(br => br.Recipient)
                .OrderByDescending(br => br.RequestDate)
                .ToListAsync();

            return View("Index", requests);
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var requests = await _context.BloodRequests
                .Include(br => br.Recipient)
                .ToListAsync();

            return View("All", requests);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var recipients = _context.Recipients?
                .Where(r => !string.IsNullOrWhiteSpace(r.FullName))
                .OrderBy(r => r.FullName)
                .ToList() ?? new List<Recipient>();

            ViewBag.RecipientId = new SelectList(recipients, "RecipientId", "FullName");
            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecipientId,BloodGroup,Quantity,Location,Message,RequestDate,Status")] BloodRequest bloodRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bloodRequest);
                await _context.SaveChangesAsync();
                TempData["ToastMessage"] = "Blood request created successfully.";
                return RedirectToAction(nameof(Index));
            }

            var recipients = _context.Recipients?
                .Where(r => !string.IsNullOrWhiteSpace(r.FullName))
                .OrderBy(r => r.FullName)
                .ToList() ?? new List<Recipient>();

            ViewBag.RecipientId = new SelectList(recipients, "RecipientId", "FullName", bloodRequest.RecipientId);
            return View("Create", bloodRequest);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var bloodRequest = await _context.BloodRequests.FindAsync(id);
            if (bloodRequest == null) return NotFound();

            var recipients = await _context.Recipients
                .Where(r => !string.IsNullOrWhiteSpace(r.FullName))
                .OrderBy(r => r.FullName)
                .ToListAsync();

            ViewBag.RecipientId = new SelectList(recipients, "RecipientId", "FullName", bloodRequest.RecipientId);
            return View("Edit", bloodRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RecipientId,BloodGroup,Quantity,Location,Message,RequestDate,Status")] BloodRequest bloodRequest)
        {
            if (id != bloodRequest.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bloodRequest);
                    await _context.SaveChangesAsync();
                    TempData["ToastMessage"] = "Blood request updated.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await _context.BloodRequests.AnyAsync(e => e.Id == bloodRequest.Id);
                    if (!exists) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var recipients = await _context.Recipients
                .Where(r => !string.IsNullOrWhiteSpace(r.FullName))
                .OrderBy(r => r.FullName)
                .ToListAsync();

            ViewBag.RecipientId = new SelectList(recipients, "RecipientId", "FullName", bloodRequest.RecipientId);
            return View("Edit", bloodRequest);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bloodRequest = await _context.BloodRequests
                .Include(br => br.Recipient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bloodRequest == null) return NotFound();

            return View("Delete", bloodRequest);
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

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var request = await _context.BloodRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = "Approved";
            await _context.SaveChangesAsync();
            TempData["ToastMessage"] = "Request approved.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var request = await _context.BloodRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = "Rejected";
            await _context.SaveChangesAsync();
            TempData["ToastMessage"] = "Request rejected.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var request = await _context.BloodRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = newStatus;
            await _context.SaveChangesAsync();
            TempData["ToastMessage"] = $"Request status updated to {newStatus}.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Match(int? id)
        {
            if (id == null) return NotFound();

            var request = await _context.BloodRequests
                .Include(r => r.Recipient)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null) return NotFound();

            var matchedDonors = await _context.Donors
                .Where(d =>
                    d.IsEligible &&
                    d.BloodGroup.ToLower() == request.BloodGroup.ToLower() &&
                    d.Location.ToLower() == request.Location.ToLower())
                .ToListAsync();

            ViewData["Request"] = request;
            ViewData["Title"] = $"Matching Donors for Request #{id}";

            return View("Match", matchedDonors);
        }
    }
}
