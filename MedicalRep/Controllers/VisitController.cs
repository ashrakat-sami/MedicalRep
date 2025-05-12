using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicalRep.Models;
using Microsoft.AspNetCore.Identity;
using MedicalRep.ViewModels;

namespace MedicalRep.Controllers
{
    [Authorize(Roles="MedicalRep")]
    public class VisitController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<VisitController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public VisitController(
            ApplicationDbContext db,
            ILogger<VisitController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var visits = await _db.Visits
                .Include(v => v.Doctor)
                .Include(v => v.MedicalRep)
                .Include(v => v.Doctor.Specialization)
                .Where(v => v.MedicalRepId == currentUser.Id)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();

            return View(visits);
        }
[HttpGet]
public async Task<IActionResult> Create()
{
    try
    {
        var currentUser = await _userManager.GetUserAsync(User);
        
        // Get only doctors (users with SpecializationId)
        var doctors = await _db.Users
            .Include(u => u.Specialization) // Include Specialization for the name
            .Where(u => u.SpecializationId != null && u.EmailConfirmed && !u.IsDeleted)
            .OrderBy(u => u.LName)
            .ThenBy(u => u.FName)
            .ToListAsync();

        var doctorList = doctors.Select(d => new {
            Id = d.Id,
            Name = $"Dr. {d.FName} {d.LName} ({d.Specialization?.Name ?? "No Specialty"})"
        }).ToList();

        var specializations = await _db.Specializations
            .OrderBy(s => s.Name)
            .ToListAsync();

        ViewBag.Doctors = new SelectList(doctorList, "Id", "Name");
        ViewBag.Specializations = new SelectList(specializations, "Id", "Name");
        ViewBag.CurrentUserId = currentUser.Id;

        return View(new VisitViewModel());
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error loading Create Visit page");
        return View("Error");
    }
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(VisitViewModel model)
{
    try
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (!ModelState.IsValid)
        {
            await RepopulateDropdowns(currentUser.Id);
            return View(model);
        }

        // Handle new doctor registration if selected
        if (model.IsNewDoctor)
        {
            var newDoctor = new ApplicationUser
            {
                UserName = model.NewDoctorEmail,
                Email = model.NewDoctorEmail,
                FName = model.NewDoctorFirstName,
                LName = model.NewDoctorLastName,
                PhoneNumber = model.NewDoctorPhone,
                Location = model.NewDoctorLocation,
                SpecializationId = model.NewDoctorSpecializationId,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newDoctor, "DefaultPassword123!");
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                await RepopulateDropdowns(currentUser.Id);
                return View(model);
            }

            // Assign doctor role
            await _userManager.AddToRoleAsync(newDoctor, "Doctor");
            model.DoctorId = newDoctor.Id;
        }

        var visit = new Visit
        {
            DoctorId = model.DoctorId,
            MedicalRepId = currentUser.Id,
            VisitDate = model.VisitDate,
            VisitNotes = model.VisitNotes,
            Status = VisitStatus.Scheduled,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Visits.Add(visit);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating new visit");
        ModelState.AddModelError("", "An error occurred while saving the visit.");
        await RepopulateDropdowns((await _userManager.GetUserAsync(User)).Id);
        return View(model);
    }
}

private async Task RepopulateDropdowns(string currentUserId)
{
    var doctors = await _db.Users
        .Include(u => u.Specialization)
        .Where(u => u.SpecializationId != null && u.EmailConfirmed && !u.IsDeleted)
        .OrderBy(u => u.LName)
        .ThenBy(u => u.FName)
        .ToListAsync();

    var doctorList = doctors.Select(d => new {
        Id = d.Id,
        Name = $"Dr. {d.FName} {d.LName} ({d.Specialization?.Name ?? "No Specialty"})"
    }).ToList();

    var specializations = await _db.Specializations
        .OrderBy(s => s.Name)
        .ToListAsync();

    ViewBag.Doctors = new SelectList(doctorList, "Id", "Name");
    ViewBag.Specializations = new SelectList(specializations, "Id", "Name");
    ViewBag.CurrentUserId = currentUserId;
}

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var visit = await _db.Visits
                .Include(v => v.Doctor)
                .FirstOrDefaultAsync(v => v.Id == id && v.MedicalRepId == currentUser.Id);

            if (visit == null)
            {
                return NotFound();
            }

            var model = new VisitEditViewModel
            {
                Id = visit.Id,
                DoctorId = visit.DoctorId,
                VisitDate = visit.VisitDate,
                ActualVisitDate = visit.ActualVisitDate,
                VisitNotes = visit.VisitNotes,
                Feedback = visit.Feedback,
                Status = visit.Status,
                DoctorName = $"{visit.Doctor.FName} {visit.Doctor.LName}"
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, VisitEditViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var visit = await _db.Visits
                .FirstOrDefaultAsync(v => v.Id == id && v.MedicalRepId == currentUser.Id);

            if (visit == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                visit.VisitDate = model.VisitDate;
                visit.ActualVisitDate = model.ActualVisitDate;
                visit.VisitNotes = model.VisitNotes;
                visit.Feedback = model.Feedback;
                visit.Status = model.Status;
                visit.UpdatedAt = DateTime.UtcNow;

                _db.Visits.Update(visit);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            foreach(var key in ModelState.Keys)
            {
                var error = ModelState[key].Errors.FirstOrDefault();
                if (error != null)
                {
                    _logger.LogWarning("Model state error for {Key}: {Error}", key, error.ErrorMessage);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var visit = await _db.Visits
                .Include(v => v.Doctor)
                .Include(v => v.Doctor.Specialization)
                .Include(v => v.MedicalRep)
                .FirstOrDefaultAsync(v => v.Id == id && v.MedicalRepId == currentUser.Id);

            if (visit == null)
            {
                return NotFound();
            }

            return View(visit);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User); 
            var visit = await _db.Visits
                .Include(v => v.Doctor)
                .FirstOrDefaultAsync(v => v.Id == id && v.MedicalRepId == currentUser.Id);

            if (visit == null)
            {
                return NotFound();
            }

            return View(visit);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var visit = await _db.Visits
                .FirstOrDefaultAsync(v => v.Id == id && v.MedicalRepId == currentUser.Id);

            if (visit == null)
            {
                return NotFound();
            }

            _db.Visits.Remove(visit);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }

   
}