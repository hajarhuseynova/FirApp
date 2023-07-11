using Fir.App.Services.Interfaces;
using Fir.App.ViewModels;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace Fir.App.Controllers
{
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;
        private readonly IMailService _mailService;

        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, SignInManager<AppUser> signinManager, IMailService mailService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signinManager = signinManager;
            _mailService = mailService;
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
            //appUser.EmailConfirmed = true;
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

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

            var link = Url.Action(action: "VerifyEmail", controller: "account",
              values: new { token = token, email = appUser.Email },
              protocol: Request.Scheme);

            await _mailService.Send("hajarih@code.edu.az", appUser.Email, link, "Verify email");
          
            TempData["register"] = "Please,verify your email";
            return RedirectToAction("index","home");
        }

        public async Task<IActionResult> VerifyEmail(string token,string email)
        {
            var user= await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                NotFound();
            }
            await _userManager.ConfirmEmailAsync(user,token);
            await _signinManager.SignInAsync(user, true);
            return RedirectToAction(nameof(Info));

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
        [Authorize]
        public async Task<IActionResult> Update()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            UserUpdateViewModel userUpdateView = new UserUpdateViewModel
            {
                Name = user.UserName,
                Surname = user.Surname,
                Email = user.Email,
                UserName = user.UserName
            };
            return View(userUpdateView);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(UserUpdateViewModel model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (user == null)
            {
                return NotFound();
            }
            user.Name = model.UserName;
            user.Surname = model.Surname;
            user.UserName = model.UserName;
            user.Email = model.Email;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }
            await _signinManager.SignInAsync(user,true);
            TempData["Updateuser"] = "ok";
            return RedirectToAction(nameof(Info));
        }
        [HttpGet]
        public async Task<IActionResult> ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string? email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            UriBuilder uriBuilder = new UriBuilder();

            var link = Url.Action(action: "resetpassword", controller: "account",
                values: new { token = token, email = email },
                protocol: Request.Scheme);

            await _mailService.Send("hajarih@code.edu.az", user.Email, link, "Reset password");
            return RedirectToAction("index","home");
        }
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token,string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };
            return View(resetPasswordViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null)
            {
                return NotFound();
            }
           var result= await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
            if (!result.Succeeded)
            {
                return Json(result.Errors);
            }
            return RedirectToAction("login", "account");
          
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
