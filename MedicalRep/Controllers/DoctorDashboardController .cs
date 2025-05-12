using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalRep.Models;
using MedicalRep.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalRep.Controllers
{
   
    public class DoctorDashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DoctorDashboardController> _logger;

        public DoctorDashboardController(
            UserManager<ApplicationUser> userManager,
            ILogger<DoctorDashboardController> logger,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var model = new DoctorDashboardViewModel
            {
                Doctor = user,
                Specialization = user.Specialization,
                TopRatedProducts = await _db.Products
                    .Where(p => p.Rate != null)
                    .OrderByDescending(p => p.Rate)
                    .Take(5)
                    .ToListAsync(),
                RecentReviews = await _db.Reviews
                    .Where(r => r.DoctorId == user.Id)
                    .OrderByDescending(r => r.ReviewDate)
                    .Take(5)
                    .ToListAsync(),
                UpcomingVisits = await _db.Visits
                    .Where(v => v.DoctorId == user.Id &&
                               v.VisitDate >= DateTime.Today &&
                               v.Status != VisitStatus.Cancelled &&
                               v.Status != VisitStatus.Completed)
                    .OrderBy(v => v.VisitDate)
                    .Take(5)
                    .ToListAsync(),
                CompletedVisitsCount = await _db.Visits
                    .CountAsync(v => v.DoctorId == user.Id &&
                                   v.Status == VisitStatus.Completed)
            };

            return View(model);
        }

        public async Task<IActionResult> ProductsBySpecialization()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null || user.SpecializationId == null)
                {
                    return View(new List<Product>());
                }

                var specializationIduser = user.SpecializationId.Value;

                var products = await _db.Products
                    .Where(p => p.ProductSpecializations
                        .Any(ps => ps.SpecializationId == specializationIduser))
                    .Include(p => p.Category) // Add this line to include Category
                    .ToListAsync();

                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error accured");
                return View(new List<Product>());
            }
        }
        public async Task<IActionResult> VisitHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            var visits = await _db.Visits
                .Where(v => v.DoctorId == user.Id)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();

            return View(visits);
        }

        public async Task<IActionResult> Details(string id)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .ThenInclude(r => r.Doctor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> ReviewProduct(string id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new ReviewViewModel
            {
                ProductId = id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewProduct(ReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var review = new Review
                {
                    DoctorId = user.Id,
                    ProductId = model.ProductId,
                    Comment = model.Comment,
                    ReviewDate = DateTime.UtcNow
                };

                _db.Reviews.Add(review);
                await _db.SaveChangesAsync();

                // Update product rating (optional)
                await UpdateProductRating(model.ProductId);

                return RedirectToAction("ProductsBySpecialization");
            }

            return View(model);
        }

        private async Task UpdateProductRating(string productId)
        {
            var product = await _db.Products
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product != null && product.Reviews.Any())
            {
                product.Rate = (decimal)product.Reviews.Average(r => r.Rating);
                await _db.SaveChangesAsync();
            }
        }
    }
}