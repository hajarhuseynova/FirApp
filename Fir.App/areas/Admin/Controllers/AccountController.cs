using Fir.App.ViewModels;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Fir.App.areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;

        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, SignInManager<AppUser> signinManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signinManager = signinManager;
        }

        public async Task<IActionResult> AdminCreate()
        {
            AppUser SuperAdmin = new AppUser
            {
                Name = "SuperAdmin",
                Surname = "SuperAdmin",
                Email = "SuperAdmin@Mail.ru",
                UserName = "SuperAdmin"
            };
            await _userManager.CreateAsync(SuperAdmin, "Admin123@");
            AppUser Admin = new AppUser
            {
                Name = "Admin",
                Surname = "Admin",
                Email = "Admin@Mail.ru",
                UserName = "Admin"
            };
            await _userManager.CreateAsync(Admin, "Admin123@");

            await _userManager.AddToRoleAsync(SuperAdmin, "SuperAdmin");
            await _userManager.AddToRoleAsync(Admin, "Admin");
            return Json("ok");
        }
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
         
            AppUser appUser =await _userManager.FindByNameAsync(loginViewModel.UserName);
            if(appUser == null)
            {
                ModelState.AddModelError("", "username or password is incorret");
                return View();
            }
          var result=  await _signinManager.PasswordSignInAsync(appUser,loginViewModel.Password,loginViewModel.isRememberMe,true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "your account blocked for 5 minutes");
                    return View();
                }
                ModelState.AddModelError("", "username or password is incorret");
                return View();
            }
            return RedirectToAction("index", "home");

        }
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();    

            return RedirectToAction("index", "home");
        }

     
    }
}
