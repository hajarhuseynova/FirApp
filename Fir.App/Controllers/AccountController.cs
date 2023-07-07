using Fir.App.ViewModels;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Fir.App.Controllers
{
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
        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }
            AppUser appUser = new AppUser();
            appUser.Email = registerViewModel.Email;
            appUser.Name = registerViewModel.Name;
            appUser.Surname = registerViewModel.Surname;    
            appUser.UserName = registerViewModel.UserName;
           IdentityResult identityResult= await _userManager.CreateAsync(appUser,registerViewModel.Password);
            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
                return View(registerViewModel);
            }
            await _userManager.AddToRoleAsync(appUser, "User");
            return RedirectToAction("index","home");
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
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();    

            return RedirectToAction("index", "home");
        }

        public async Task<IActionResult> Info()
        {
            string UserName=User.Identity.Name;
            AppUser appUser=await _userManager.FindByNameAsync(UserName);

            return View(appUser);
        }
        //public async Task<IActionResult> CreateRole()
        //{
        //    IdentityRole identityRole1= new IdentityRole { Name="SuperAdmin"};
        //    IdentityRole identityRole2 = new IdentityRole { Name = "Admin" };
        //    IdentityRole identityRole3 = new IdentityRole { Name = "User" };

        //    await _roleManager.CreateAsync(identityRole1);

        //    await _roleManager.CreateAsync(identityRole2);

        //    await _roleManager.CreateAsync(identityRole3);

        //    return Json("ok");
        //}
    }
}
