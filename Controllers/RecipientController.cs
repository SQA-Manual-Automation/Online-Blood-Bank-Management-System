using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using BloodBankSystem.Data.Abstractions;
using BloodBankSystem.Models;
using BloodBankSystem.Models.ViewModels;
using BloodBankSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodBankSystem.Controllers
{
    public class RecipientController : Controller
    {
        private readonly IRecipientRepository _recipientRepository;
        private readonly ApplicationDbContext _context;

        public RecipientController(IRecipientRepository recipientRepository, ApplicationDbContext context)
        {
            _recipientRepository = recipientRepository;
            _context = context;
            
            Console.WriteLine("[DEBUG] Connected to DB: " + _context.Database.GetDbConnection().ConnectionString);
        }

        [HttpGet]
        public IActionResult Index() => RedirectToAction(nameof(Dashboard));

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var recipients = await _recipientRepository.GetAllRecipientsAsync();
            return View(recipients);
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Recipient recipient)
        {
            if (!ModelState.IsValid)
                return View(recipient);

            if (recipient.RequiredDate < DateTime.Today)
            {
                ModelState.AddModelError("RequiredDate", "Required date cannot be in the past.");
                return View(recipient);
            }

            recipient.ApplicationUserId = Guid.NewGuid().ToString();

            var rowsAffected = await _recipientRepository.AddAsync(recipient);
            if (rowsAffected > 0)
                return RedirectToAction(nameof(Login));

            ModelState.AddModelError("", "Registration failed. Please try again.");
            return View(recipient);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            using var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            var query = @"
                SELECT RecipientId, FullName
                FROM Recipients
                WHERE Email = @Email AND Password = @Password;
            ";

            using var command = connection.CreateCommand();
            command.CommandText = query;

            var emailParam = command.CreateParameter();
            emailParam.ParameterName = "@Email";
            emailParam.Value = email;
            command.Parameters.Add(emailParam);

            var passwordParam = command.CreateParameter();
            passwordParam.ParameterName = "@Password";
            passwordParam.Value = password;
            command.Parameters.Add(passwordParam);

            using var reader = await command.ExecuteReaderAsync();
            if (reader.Read())
            {
                var recipientId = reader.GetInt32(0);
                HttpContext.Session.SetInt32("RecipientId", recipientId);
                return RedirectToAction(nameof(Dashboard));
            }

            ViewBag.Error = "Invalid credentials.";
            ViewData["Email"] = email;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var recipientId = HttpContext.Session.GetInt32("RecipientId");
            if (recipientId == null)
                return RedirectToAction(nameof(Login));

            var recipient = await _recipientRepository.GetRecipientByIdAsync(recipientId.Value);
            if (recipient == null)
                return NotFound();

            var requests = await _recipientRepository.GetBloodRequestsByRecipientIdAsync(recipientId.Value);
            var viewModel = new RecipientDashboardViewModel
            {
                RecipientId = recipient.RecipientId,
                RecipientName = recipient.FullName ?? "Unknown",
                TotalRequests = requests.Count,
                PendingRequests = requests.Count(r => r.Status == "Pending"),
                ApprovedRequests = requests.Count(r => r.Status == "Approved"),
                RequestHistory = requests
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var recipientId = HttpContext.Session.GetInt32("RecipientId");
            if (recipientId == null)
                return RedirectToAction(nameof(Login));

            var recipient = await _recipientRepository.GetRecipientByIdAsync(recipientId.Value);
            if (recipient == null)
                return NotFound();

            var requests = await _recipientRepository.GetBloodRequestsByRecipientIdAsync(recipientId.Value);
            ViewBag.Requests = requests;
            return View(recipient);
        }

        [HttpGet]
        public IActionResult RequestBlood() => View(new BloodRequest());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestBlood(BloodRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var recipientId = HttpContext.Session.GetInt32("RecipientId");
            if (recipientId == null)
                return RedirectToAction(nameof(Login));

            request.RecipientId = recipientId.Value;
            request.Status = "Pending";
            request.RequestDate = DateTime.UtcNow;
            request.RequestedAt = DateTime.UtcNow;

            var success = await _recipientRepository.PostBloodRequestAsync(request);
            if (success)
            {
                TempData["ToastMessage"] = "Blood request submitted successfully.";
                return RedirectToAction(nameof(Dashboard));
            }

            ModelState.AddModelError("", "Request failed. Please try again.");
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> EditBloodRequest(int id)
        {
            var recipientId = HttpContext.Session.GetInt32("RecipientId");
            if (recipientId == null)
                return RedirectToAction(nameof(Login));

            var request = await _recipientRepository.GetBloodRequestByIdAsync(id);
            if (request == null || request.RecipientId != recipientId.Value)
                return NotFound();

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBloodRequest(BloodRequest updatedRequest)
        {
            var recipientId = HttpContext.Session.GetInt32("RecipientId");
            if (recipientId == null || updatedRequest.RecipientId != recipientId.Value)
                return RedirectToAction(nameof(Login));

            if (!ModelState.IsValid)
                return View(updatedRequest);

            var existingRequest = await _recipientRepository.GetBloodRequestByIdAsync(updatedRequest.Id);
            if (existingRequest == null || existingRequest.RecipientId != recipientId.Value)
                return NotFound();

            // Preserve original status
            updatedRequest.Status = existingRequest.Status;

            var success = await _recipientRepository.UpdateBloodRequestAsync(updatedRequest);

            if (success)
            {
                TempData["ToastMessage"] = "Blood request updated successfully.";
                return RedirectToAction(nameof(Dashboard));
            }

            ModelState.AddModelError("", "Update failed. Please try again.");
            return View(updatedRequest);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteBloodRequest(int id)
        {
            var recipientId = HttpContext.Session.GetInt32("RecipientId");
            if (recipientId == null)
                return RedirectToAction(nameof(Login));

            var request = await _recipientRepository.GetBloodRequestByIdAsync(id);
            if (request == null || request.RecipientId != recipientId.Value)
                return NotFound();

            return View(request);
        }

        [HttpPost, ActionName("DeleteBloodRequest")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDeleteBloodRequest(int id)
        {
            var recipientId = HttpContext.Session.GetInt32("RecipientId");
            if (recipientId == null)
                return RedirectToAction(nameof(Login));

            var request = await _recipientRepository.GetBloodRequestByIdAsync(id);
            if (request == null || request.RecipientId != recipientId.Value)
                return NotFound();

            var success = await _recipientRepository.DeleteBloodRequestAsync(id);
            if (success)
            {
                TempData["ToastMessage"] = "Blood request deleted successfully.";
                return RedirectToAction(nameof(Dashboard));
            }

            ModelState.AddModelError("", "Deletion failed. Please try again.");
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> MatchDonors()
        {
            var recipientId = HttpContext.Session.GetInt32("RecipientId");
            if (recipientId == null)
                return RedirectToAction(nameof(Login));

            var recipient = await _recipientRepository.GetRecipientByIdAsync(recipientId.Value);
            if (recipient == null)
                return NotFound();

            var matchingDonors = await _context.Donors
                .Where(d => d.BloodGroup == recipient.BloodGroup)
                .ToListAsync();

            var viewModel = new MatchResultViewModel
            {
                RecipientName = recipient.FullName,
                RequestedBloodGroup = recipient.BloodGroup,
                MatchingDonors = matchingDonors
            };

            return View(viewModel);
        }
    }
}