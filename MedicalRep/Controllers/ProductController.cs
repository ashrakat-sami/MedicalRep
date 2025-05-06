using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicalRep.Models;
using System.IO;
using MedicalRep;

namespace MedicalRep.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ProductController> _logger;
        public ProductController(ILogger<ProductController> logger,ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _logger = logger;
            _context = context;
            _environment = environment;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

       

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Helpers

        private void PopulateCategories(object selectedCategory = null)
        {
            var categories = _context.Categories
                .OrderBy(c => c.Name)
                .ToList();

            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", selectedCategory);
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return "/images/" + fileName;
        }


        // GET: Product/Create
        public IActionResult Create()
        {
            _logger.LogInformation("GET Create action called");
            var categories = _context.Categories.ToList();
            _logger.LogInformation($"Found {categories.Count} categories");

            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile Image)
        {
            _logger.LogInformation("POST Create action called");
            _logger.LogInformation($"Received product data: {Newtonsoft.Json.JsonConvert.SerializeObject(product)}");

            if (ModelState.IsValid)
            {
                // Load the category to satisfy navigation property requirement
                //product.Category = await _context.Categories.FindAsync(product.CategoryId);

                _logger.LogInformation("ModelState is valid");
                product.Id = Guid.NewGuid().ToString();

                if (Image != null && Image.Length > 0)
                {
                    _logger.LogInformation("Processing image upload");
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "products");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(stream);
                    }

                    product.Image = "/images/products/" + fileName;
                }
                else
                {
                    _logger.LogInformation("No image uploaded");
                }

                _logger.LogInformation($"Attempting to add product with CategoryId: {product.CategoryId}");
                _context.Add(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product successfully created");
                return RedirectToAction(nameof(Index));
            }

            // Log all validation errors
            _logger.LogWarning("ModelState is invalid. Errors:");
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                foreach (var error in state.Errors)
                {
                    _logger.LogError($"Key: {key}, Error: {error.ErrorMessage}");
                }
            }

            // Log the actual received CategoryId
            _logger.LogInformation($"Received CategoryId value: {product.CategoryId}");

            var categories = _context.Categories.ToList();
            _logger.LogInformation($"Available categories count: {categories.Count}");

            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            _logger.LogInformation($"GET Edit action called for id: {id}");
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning($"Product with id {id} not found");
                return NotFound();
            }

            _logger.LogInformation($"Product found with CategoryId: {product.CategoryId}");
            PopulateCategories(product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Product product, IFormFile? imageFile, bool removeImage = false)
        {
            _logger.LogInformation($"POST Edit action called for id: {id}");
            _logger.LogInformation($"Received product data: {Newtonsoft.Json.JsonConvert.SerializeObject(product)}");

            if (id != product.Id)
            {
                _logger.LogWarning($"ID mismatch: {id} != {product.Id}");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid. Errors:");
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        _logger.LogError($"Key: {key}, Error: {error.ErrorMessage}");
                    }
                }

                _logger.LogInformation($"Received CategoryId value: {product.CategoryId}");
                PopulateCategories(product.CategoryId);
                Response.StatusCode = 400;
                return View(product);
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                _logger.LogWarning($"Product with id {id} not found");
                return NotFound();
            }

            // Image logic
            if (removeImage)
            {
                _logger.LogInformation("Removing existing image");
                existingProduct.Image = null;
            }
            else if (imageFile != null && imageFile.Length > 0)
            {
                _logger.LogInformation("Processing new image upload");
                existingProduct.Image = await SaveImageAsync(imageFile);
            }

            // Log before updating
            _logger.LogInformation($"Updating product. Old CategoryId: {existingProduct.CategoryId}, New CategoryId: {product.CategoryId}");

            // Update properties
            existingProduct.Name = product.Name;
            existingProduct.Rate = product.Rate;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;
            existingProduct.Manufacturer = product.Manufacturer;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.DosageForm = product.DosageForm;
            existingProduct.Strength = product.Strength;
            existingProduct.ExpiryDate = product.ExpiryDate;
            existingProduct.Indications = product.Indications;
            existingProduct.SideEffects = product.SideEffects;
            existingProduct.Contraindications = product.Contraindications;
            existingProduct.Ingredients = product.Ingredients;
            existingProduct.IsPrescriptionOnly = product.IsPrescriptionOnly;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product successfully updated");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving product changes");
                throw;
            }
        }
    }
}
