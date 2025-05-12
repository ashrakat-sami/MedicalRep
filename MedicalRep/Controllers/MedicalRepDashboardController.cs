using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using MedicalRep.Models;
using MedicalRep.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using MedicalRep.Hubs;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Query;
using MedicalRep.ViewModels;
using MedicalRep.Models;

namespace MedicalRep.Controllers

{
    [Authorize(Roles = "MedicalRep")]
    public class MedicalRepDashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<MedicalRepDashboardController> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public MedicalRepDashboardController(
            UserManager<ApplicationUser> userManager,
            ILogger<MedicalRepDashboardController> logger,
            ApplicationDbContext db,
            IHubContext<NotificationHub> hubContext)
        {
            _db = db;
            _userManager = userManager;
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogWarning("User not found.");
                return RedirectToAction("Login", "Account");
            }

            if (!await _userManager.IsInRoleAsync(user, "MedicalRep"))
            {
                _logger.LogWarning("User is not a MedicalRep.");
                return Forbid();
            }

            try
            {
                var dashboardData = await _db.Users
                    .Where(u => u.Id == user.Id)
                    .Select(u => new
                    {
                        User = u,
                        Products = _db.Products
                            .Include(p => p.Category)
                            .OrderByDescending(p => p.Rate)
                            .Take(5)
                            .ToList(),
                        Specializations = _db.Specializations
                            .Include(s => s.Doctors)
                            .Select(s => new DoctorsBySpecialization
                            {
                                Specialization = s,
                                DoctorCount = s.Doctors.Count
                            })
                            .ToList(),
                        RecentVisits = _db.Visits
                            .Where(v => v.MedicalRepId == u.Id)
                            .OrderByDescending(v => v.VisitDate)
                            .Take(5)
                            .Include(v => v.Doctor)
                            .ToList(),
                        TotalDoctors = _db.Users.Count(u => u.SpecializationId != null)
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (dashboardData == null)
                {
                    return NotFound();
                }

                var model = new MedicalRepDashboardViewModel
                {
                    MedicalRep = dashboardData.User,
                    Products = dashboardData.Products,
                    DoctorsBySpecialization = dashboardData.Specializations,
                    RecentVisits = dashboardData.RecentVisits,
                    TotalAllDoctors = dashboardData.TotalDoctors,
                    VisitStats = new VisitStats
                    {
                        TotalVisits = await _db.Visits.CountAsync(v => v.MedicalRepId == user.Id),
                        ThisMonthVisits = await _db.Visits.CountAsync(v =>
                            v.MedicalRepId == user.Id &&
                            v.VisitDate.Month == DateTime.Today.Month &&
                            v.VisitDate.Year == DateTime.Today.Year),
                        PlannedVisits = await _db.Visits.CountAsync(v =>
                            v.MedicalRepId == user.Id &&
                            v.VisitDate >= DateTime.Today)
                    }
                };

                ViewBag.NotificationHubUrl = "/notificationHub";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                return StatusCode(500, "An error occurred while loading dashboard data");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest("Search query cannot be empty");
                }

                var results = new SearchResultViewModel
                {
                    Doctors = await _db.Users
                        .Where(u => u.SpecializationId != null &&
                                   (u.FName.Contains(query) ||
                                    u.LName.Contains(query) ||
                                    u.Specialization.Name.Contains(query)))
                        .Select(u => new DoctorSearchResult
                        {
                            Id = u.Id,
                            Name = $"{u.FName} {u.LName}",
                            Specialization = u.Specialization.Name,
                            Location = u.Location
                        })
                        .Take(5)
                        .ToListAsync(),
                    Products = await _db.Products
                        .Where(p => p.Name.Contains(query) ||
                                   p.Description.Contains(query))
                        .Select(p => new ProductSearchResult
                         {
                             Id = p.Id,
                             Name = p.Name,
                             Rate = p.Rate ?? 0,
                             Description = p.Description
                         })
                        .Take(5)
                        .ToListAsync()
                };

                return Json(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during search");
                return StatusCode(500, "An error occurred during search");
            }
        }

        public async Task<IActionResult> DoctorsList(int? specializationId)
        {
            try
            {
                var doctors = await _db.Users
                  .Where(u => u.SpecializationId != null && (!specializationId.HasValue || u.SpecializationId == specializationId))
                  .Include(u => u.Specialization)
                  .Select(u => new
                  {
                      u.Id,
                      u.FName,
                      u.LName,
                      u.Email,
                      u.PhoneNumber,
                      Specialization = u.Specialization.Name,
                      u.Location
                  })
                  .ToListAsync();


                return Json(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading doctors list");
                return StatusCode(500, "An error occurred while loading doctors list");
            }
        }

        public async Task<IActionResult> VisitPlanner()
        {
            var user = await _userManager.GetUserAsync(User);

            try
            {
                var model = new VisitPlannerViewModel
                {
                    Doctors = await _db.Users
                        .Where(u => u.SpecializationId != null)
                        .Include(u => u.Specialization)
                        .ToListAsync(),
                    PlannedVisits = await _db.Visits
                        .Where(v => v.MedicalRepId == user.Id &&
                                    v.VisitDate >= DateTime.Today)
                        .OrderBy(v => v.VisitDate)
                        .Include(v => v.Doctor)
                        .ToListAsync()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading visit planner");
                return StatusCode(500, "An error occurred while loading visit planner");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification(string message)
        {
            var user = await _userManager.GetUserAsync(User);
            await _hubContext.Clients.Group(user.UserName).SendAsync("ReceiveNotification", message);
            return Ok();
        }
    }
}