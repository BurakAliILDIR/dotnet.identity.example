using IdentityExample.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using IdentityExample.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using IdentityExample.Web.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NuGet.Common;

namespace IdentityExample.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var identityResult = await _userManager.CreateAsync(new AppUser()
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                City = string.Empty,
            }, password: request.Password);


            if (identityResult.Succeeded)
            {
                TempData["SuccessMessage"] = "Başarıyla kayıt oldunuz.";

                return RedirectToAction(nameof(SignUp));
            }

            foreach (IdentityError error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel request, string? returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Action("Index", "Home");

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Email veya parola yanlış.");
                return View();
            }

            var result =
                await _signInManager.PasswordSignInAsync(user.UserName, request.Password, request.RememberMe, true);

            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty,
                    "Çok fazla yanlış deneme yaptığınız için 3 dakika engellendiniz.");
                return View();
            }

            var count = await _userManager.GetAccessFailedCountAsync(user);

            ModelState.AddModelErrors(new List<string>()
            {
                "Email veya parola yanlış.", $"Başarısız giriş sayısı: {count}"
            });

            return View();
        }


        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel request)
        {
            // https://localhost:7067

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Bu email adresine ait kullanıcı bulunamamıştır.");
                return View();
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var passwordResetLink =
                Url.Action("ResetPassword", "Home", new { userId = user.Id, Token = passwordResetToken });

            // email service

            TempData["SuccessMessage"] = "Şifre sıfırlama linki email adresinize gönderilmiştir.";

            return RedirectToAction("SignIn", "Home");
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            throw new NotImplementedException();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}