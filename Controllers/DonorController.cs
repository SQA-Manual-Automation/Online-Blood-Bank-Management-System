using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using BloodBankSystem.Data;
using BloodBankSystem.Models;

namespace BloodBankSystem.Controllers
{
    public class DonorsController : Controller
    {
        private readonly ApplicationDbContext db;

        public DonorsController(ApplicationDbContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            Console.WriteLine($"Login attempt: {email}");

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Email and password are required.";
                ViewData["Email"] = email;
                return View();
            }

            var donor = db.Donors
                .AsEnumerable()
                .FirstOrDefault(x =>
                    !string.IsNullOrEmpty(x.Email) &&
                    x.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                    x.Password == password);

            if (donor == null)
            {
                Console.WriteLine("Login failed: Invalid credentials.");
                ViewBag.Error = "Invalid credentials.";
                ViewData["Email"] = email;
                return View();
            }

            HttpContext.Session.SetInt32("DonorId", donor.DonorId);
            HttpContext.Session.SetString("DonorName", donor.FullName);
            TempData["ToastMessage"] = $"Welcome, {donor.FullName}!";
            Console.WriteLine("Login successful.");
            return RedirectToAction(nameof(DonorIndex));
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var donors = db.Donors.ToList();
                Console.WriteLine($"Loaded {donors.Count} donors.");
                return View(donors);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR loading donor list: " + ex.Message);
                TempData["ToastMessage"] = "Failed to load donor list.";
                return View(Enumerable.Empty<Donor>());
            }
        }

        [HttpGet]
        public IActionResult DonorIndex()
        {
            var donorId = HttpContext.Session.GetInt32("DonorId");
            if (donorId == null)
                return RedirectToAction(nameof(Login));

            var donor = db.Donors.Find(donorId.Value);
            if (donor == null)
                return NotFound("Donor not found.");

            return View(donor);
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Donor donor)
        {
            if (donor == null)
            {
                Console.WriteLine("ERROR: Donor object is null.");
                ModelState.AddModelError("", "Invalid form submission.");
                return View();
            }

            Console.WriteLine("HIT: Register POST for " + donor.Email);

            donor.ApplicationUserId = Guid.NewGuid().ToString();
            ModelState.Remove(nameof(Donor.ApplicationUserId));

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState INVALID. Errors:");
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Console.WriteLine($" - {kvp.Key}: {error.ErrorMessage}");
                    }
                }
                return View(donor);
            }

            var age = DateTime.Today.Year - donor.DateOfBirth.Year;
            if (donor.DateOfBirth > DateTime.Today.AddYears(-age)) age--;

            if (age < 18 || age > 60)
            {
                Console.WriteLine($"Validation blocked: age {age} out of range.");
                ModelState.AddModelError("DateOfBirth", "Donor must be between 18 and 60 years old.");
                return View(donor);
            }

            if (donor.LastDonationDate is DateTime lastDate)
            {
                var daysSinceLastDonation = (DateTime.Today - lastDate).TotalDays;
                if (daysSinceLastDonation < 90)
                {
                    Console.WriteLine($"Validation blocked: {daysSinceLastDonation} days since last donation (<90).");
                    ModelState.AddModelError("LastDonationDate", "Minimum 90 days required since last donation.");
                    return View(donor);
                }
            }

            try
            {
                Console.WriteLine("Adding donor to context: " + donor.Email);
                db.Donors.Add(donor);
                db.SaveChanges();
                Console.WriteLine("SaveChanges COMPLETED.");

                TempData["ToastMessage"] = "Donor registered successfully.";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR during SaveChanges: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("INNER: " + ex.InnerException.Message);

                ModelState.AddModelError("", "Something went wrong while saving. Please try again.");
                return View(donor);
            }
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Donor donor)
        {
            if (!ModelState.IsValid)
                return View(donor);

            donor.ApplicationUserId = Guid.NewGuid().ToString();

            // ✅ Fill required fields if missing
            if (string.IsNullOrWhiteSpace(donor.Email))
                donor.Email = $"{donor.FullName.Replace(" ", "").ToLower()}@bloodbank.com";

            if (string.IsNullOrWhiteSpace(donor.Password))
                donor.Password = "Donor@123";

            try
            {
                db.Donors.Add(donor);
                db.SaveChanges();
                TempData["ToastMessage"] = "Donor created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR during Create: " + ex.Message);
                ModelState.AddModelError("", "Failed to create donor. Try again.");
                return View(donor);
            }
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var donorId = HttpContext.Session.GetInt32("DonorId");
            if (donorId == null)
                return RedirectToAction(nameof(Login));

            var donor = db.Donors.Find(donorId.Value);
            if (donor == null)
                return NotFound("Donor not found.");

            return View(donor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Donor updatedDonor)
        {
            if (updatedDonor == null || updatedDonor.DonorId == 0)
            {
                Console.WriteLine("ERROR: Invalid donor object or missing ID.");
                return BadRequest("Invalid donor data.");
            }

            ModelState.Remove(nameof(Donor.ApplicationUserId));
            ModelState.Remove(nameof(Donor.Email));
            ModelState.Remove(nameof(Donor.Password));

            var donor = db.Donors.Find(updatedDonor.DonorId);
            if (donor == null)
            {
                Console.WriteLine("ERROR: Donor not found during Edit.");
                return NotFound("Donor not found.");
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState INVALID during Edit. Errors:");
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Console.WriteLine($" - {kvp.Key}: {error.ErrorMessage}");
                    }
                }
                return View(updatedDonor);
            }

            donor.FullName = updatedDonor.FullName;
            donor.ContactNumber = updatedDonor.ContactNumber;
            donor.Location = updatedDonor.Location;
            donor.BloodGroup = updatedDonor.BloodGroup;
            donor.DateOfBirth = updatedDonor.DateOfBirth;
            donor.LastDonationDate = updatedDonor.LastDonationDate;
            donor.IsEligible = updatedDonor.IsEligible;

            try
            {
                db.SaveChanges();
                Console.WriteLine("Profile updated successfully.");
                TempData["ToastMessage"] = "Profile updated successfully.";
                return RedirectToAction(nameof(DonorIndex));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR during profile update: " + ex.Message);
                ModelState.AddModelError("", "Failed to update profile. Try again.");
                return View(updatedDonor);
            }
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var donor = db.Donors.Find(id);
            if (donor == null)
                return NotFound();

            return View(donor);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var donor = db.Donors.Find(id);
            if (donor == null)
                return NotFound();

            db.Donors.Remove(donor);
            db.SaveChanges();

            TempData["ToastMessage"] = "Donor deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

                [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["ToastMessage"] = "You have been logged out.";
            return RedirectToAction(nameof(Login));
        }
    }
}
