using IdentityExample.Web.Models;
using IdentityExample.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityExample.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<ActionResult> IndexAsync()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            var userViewModel = new UserViewModel
            {
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
            };
            return View(userViewModel);
        }

        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("PasswordChange", "Member");
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var checkPasswordResult = await _userManager.CheckPasswordAsync(user, request.OldPassword);

            if (checkPasswordResult)
            {
                var response = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

                if (response.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                    await _signInManager.SignOutAsync();
                    await _signInManager.PasswordSignInAsync(user, request.NewPassword, true, false);

                    TempData["SuccessMessage"] = "Şifreniz başarıyla güncellendi";
                    return RedirectToAction("PasswordChange", "Member");
                }
            }

            ModelState.AddModelError(string.Empty, "Eski parolanız yanlış.");

            return View();
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
    }
}