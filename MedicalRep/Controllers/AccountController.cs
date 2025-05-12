using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MedicalRep.Models;
using MedicalRep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MedicalRep.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            ILogger<AccountController> logger,
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _db = db;
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel
            {
                RoleOptions = GetRoles()
            };

            ViewBag.Specializations = new SelectList(_db.Specializations.ToList(), "Id", "Name");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Always repopulate these before returning the view
            model.RoleOptions = GetRoles();
            ViewBag.Specializations = new SelectList(_db.Specializations.ToList(), "Id", "Name");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state during registration for {Email}", model.Email);
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FName = model.FName,
                LName = model.LName,
                City = model.City,
                Street = model.Street,
                Government = model.Government,
                IsDeleted = model.IsDeleted,
                Location = model.Location,
                SpecializationId = model.SelectedRole == "Doctor" ? model.SpecializationId : null
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User registered successfully: {Email}", model.Email);

                if (!await _roleManager.RoleExistsAsync(model.SelectedRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole(model.SelectedRole));
                }

                await _userManager.AddToRoleAsync(user, model.SelectedRole);
                await _signInManager.SignInAsync(user, isPersistent: false);

                return model.SelectedRole switch
                {
                    "Doctor" => RedirectToAction("Index", "DoctorDashboard"),
                    "MedicalRep" => RedirectToAction("Index", "MedicalRepDashboard"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        private List<SelectListItem> GetRoles()
        {
            return new List<SelectListItem>
    {
        new SelectListItem { Value = "Doctor", Text = "Doctor" },
        new SelectListItem { Value = "MedicalRep", Text = "Medical Representative" }
    };
        }


        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    foreach (var subError in error.Value.Errors)
                    {
                        _logger.LogError("ModelState error in {Field}: {Error}", error.Key, subError.ErrorMessage);
                    }
                }

                _logger.LogWarning("Invalid model state during login for {Email}", model.Email);
                return View(model);
            }


            _logger.LogInformation("Login attempt for {Email}", model.Email);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                if (user.IsDeleted)
                {
                    _logger.LogWarning("Login blocked for deactivated user {Email}", model.Email);
                }
                else
                {
                    var result = await _signInManager.PasswordSignInAsync(
                        user.UserName,
                        model.Password,
                        model.RememberMe,
                        lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Login succeeded for {Email}", model.Email);

                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else if (roles.Contains("Doctor"))
                        {
                            return RedirectToAction("Index", "DoctorDashboard");
                        }
                        else if (roles.Contains("MedicalRep"))
                        {
                            return RedirectToAction("Index", "MedicalRepDashboard");
                        }
                        else
                        {
                            _logger.LogWarning("Login failed: user has no valid role - {Email}", model.Email);
                        }

                    }
                    else
                    {
                        _logger.LogWarning("Invalid password for user {Email}", model.Email);
                    }
                }
            }
            else
            {
                _logger.LogWarning("Login failed: user not found - {Email}", model.Email);
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt or account is deactivated");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out");
            return RedirectToAction("", "Home");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ManageUsers()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
