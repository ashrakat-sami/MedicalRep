using MedicalRep.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MedicalRep.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult  Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel newUser)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser();
                user.UserName = newUser.UserName;
                user.Email = newUser.Email;
                user.PasswordHash = newUser.Password;




                IdentityResult result =await userManager.CreateAsync(user, newUser.Password);
                if (result.Succeeded)
                {
                  await  signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(newUser);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = await userManager.FindByNameAsync(loginVM.UserName);
                if (applicationUser != null)
                {
                    bool found = await userManager.CheckPasswordAsync(applicationUser, loginVM.Password);

                    if (found)
                    {
                        List<Claim> claims = new List<Claim>();
                        // claims.Add(new Claim("Address", applicationUser.Address));
                        await signInManager.SignInWithClaimsAsync(applicationUser, loginVM.RememberMe, claims);
                        return RedirectToAction("Index", "Home");

                    }
                }

                ModelState.AddModelError("", "User name and password or invalid");

            }

            return View(loginVM);
        }

      
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
       

        public IActionResult Index()
        {
            return View();
        }
    }
}
