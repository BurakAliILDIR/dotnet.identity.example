using IdentityExample.Web.Extensions;
using IdentityExample.Web.Models;
using IdentityExample.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.FileProviders;

namespace IdentityExample.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager,
            IFileProvider fileProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
        }

        public async Task<ActionResult> IndexAsync()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            var userViewModel = new UserViewModel
            {
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
                PictureUrl = currentUser.Picture
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


        public async Task<IActionResult> UserEdit()
        {
            ViewBag.genders = new SelectList(Enum.GetNames(typeof(Gender)));

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var userEditViewModel = new UserEditViewModel()
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.BirthDate,
                City = user.City,
                Gender = user.Gender,
            };


            return View(userEditViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("UserEdit", "Member");
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            user.UserName = request.UserName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.BirthDate = request.BirthDate;
            user.City = request.City;
            user.Gender = request.Gender;
            user.Gender = request.Gender;

            if (request.Picture is not null && request.Picture.Length > 0)
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");
                var randomFilename = Guid.NewGuid() + Path.GetExtension(request.Picture.FileName);

                var newPicturePath = Path.Combine(wwwrootFolder.First(x => x.Name == "pictures").PhysicalPath,
                    randomFilename);

                using (var stream = new FileStream(newPicturePath, FileMode.Create))
                {
                    await request.Picture.CopyToAsync(stream);
                }

                user.Picture = randomFilename;
            }

            var updateToUser = await _userManager.UpdateAsync(user);

            if (!updateToUser.Succeeded)
            {
                ModelState.AddModelErrors(updateToUser.Errors.Select(x => x.Description).ToList());
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(user);
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(user, true);

            TempData["SuccessMessage"] = "Bilgileriniz başarıyla güncellendi";
            return RedirectToAction("UserEdit", "Member");
        }


        public IActionResult Claims()
        {
            var userClaims = User.Claims.Select(x => new ClaimViewModel
            {
                Issuer = x.Issuer,
                Type = x.Type,
                Value= x.Value
            }).ToList();

            return View(userClaims);
        }

    }
}