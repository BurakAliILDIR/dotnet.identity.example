using IdentityExample.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using IdentityExample.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using IdentityExample.Web.Extensions;
using IdentityExample.Web.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NuGet.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IdentityExample.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;


        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
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

            IdentityResult identityResult = await _userManager.CreateAsync(new AppUser()
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                City = string.Empty,
            }, password: request.Password);


            if (identityResult.Succeeded)
            {
                var exchangeExpireClaim = new Claim("ExchangeExpireDate", DateTime.UtcNow.AddDays(10).ToString());

                var user = await _userManager.FindByEmailAsync(request.Email);

                var result = await _userManager.AddClaimAsync(user, exchangeExpireClaim);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, result.Errors.ToString());
                    return View();
                }

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
            returnUrl ??= Url.Action("Index", "Home");

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
                if (user.BirthDate.HasValue)
                {
                    await _signInManager.SignInWithClaimsAsync(user, true, new List<Claim>
                    {
                        new("birthDate", user.BirthDate.ToString())
                    });
                }

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
                Url.Action("ResetPassword", "Home", new { userId = user.Id, Token = passwordResetToken },
                    HttpContext.Request.Scheme);

            // email service

            await _emailService.SendResetPasswordEmail(passwordResetLink, user.Email);

            TempData["SuccessMessage"] = "Şifre sıfırlama linki email adresinize gönderilmiştir.";

            return RedirectToAction("SignIn", "Home");
        }


        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;

            var user = _userManager.FindByIdAsync((string)userId);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Bu email adresine ait kullanıcı bulunamamıştır.");
                return View();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
        {
            var userId = TempData["userId"];
            var token = TempData["token"];

            if (userId is null || token is null)
            {
                throw new Exception("Hata meydana geldi.");
            }

            var user = await _userManager.FindByIdAsync((string)userId);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Bu email adresine ait kullanıcı bulunamamıştır.");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(user, (string)token, request.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Şifreniz başarıyla yenilenmiştir.";
                return RedirectToAction("SignIn", "Home");
            }

            ModelState.AddModelErrors(result.Errors.Select(x => x.Description).ToList());

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}