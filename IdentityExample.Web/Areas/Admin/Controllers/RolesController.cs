using IdentityExample.Web.Areas.Admin.ViewModels;
using IdentityExample.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityExample.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;


        public RolesController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
           var roles = await _roleManager.Roles.Select(x => new RoleViewModel()
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();

            return View(roles);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleCreateViewModel request)
        {
            var result = await _roleManager.CreateAsync(new AppRole
            {
                Name = request.Name
            });

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Rol eklerken hata oluştu.");
                return View();
            }

            return RedirectToAction("Index");
        }
    }
}