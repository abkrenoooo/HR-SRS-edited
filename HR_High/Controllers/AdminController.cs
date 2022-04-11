
using BAL.Contants;
using DLL.Data;
using DLL.Models;
using DLL.ViewModel.AdminRole;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HR.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;

        public ApplicationDbContext db { get; }

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, ApplicationDbContext _db)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            db = _db;
        }

        [Authorize("Permissions.Group.View")]
        public async Task<IActionResult> Index()
        {
            var roles = await roleManager.Roles.Where(x=>x.Name!="Server").ToListAsync();
            return View(roles);
        }
        //if(AuthorizationService.AuthorizeAsync(User, Permissions.Module.Edit(Modules.Group.ToString())).Result.Succeeded)
        //[Authorize(Permissions.Products.Edit)]
        [Authorize("Permissions.Group.Create")]
        public IActionResult Create()
        {
            var allClaims = Permissions.GenerateAllPermissions();
            var allPermissions = allClaims.Select(p => new CheckBoxViewModel { DisplayValue = p }).ToList();

            var viewModel = new PermissionsFormViewModel
            {
                RoleId = null,
                RoleName = null,
                RoleCalims = allPermissions
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PermissionsFormViewModel model)
        {
            if (model.RoleName == null)
            {
                ModelState.AddModelError("RoleName", "Pleaze Enter The Name Of Group");

                return View(model);
            }
            if (await roleManager.RoleExistsAsync(model.RoleName))
            {
                ModelState.AddModelError("RoleName", "The Group is exists..! Pleaze Enter Another Name Of Group");

                return View(model);
            }

            if (model.RoleCalims.Where(p => p.IsSelected == true).Count() == 0)
            {
                ModelState.AddModelError("RoleName", "Please specify group permissions before adding");
                return View(model);
            }
            if (model.RoleCalims.Where(p => p.IsSelected == true).Count() > 0 && !await roleManager.RoleExistsAsync(model.RoleName))
            {

                await roleManager.CreateAsync(new IdentityRole(model.RoleName.Trim()));
                var allClaims = Permissions.GenerateAllPermissions().ToList();
                var roleCalims = model.RoleCalims.ToList();


                var role = await roleManager.FindByNameAsync(model.RoleName);
                
                var selectedClaims = model.RoleCalims.Where(c => c.IsSelected).ToList();

                foreach (var claim in selectedClaims)
                    await roleManager.AddClaimAsync(role, new Claim("Permission", claim.DisplayValue));


            }
            return RedirectToAction(nameof(Index));
        }
        [Authorize("Permissions.Group.Edit")]
        public async Task<IActionResult> EditPermissions(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
                return NotFound();

            var roleClaims = roleManager.GetClaimsAsync(role).Result.Select(c => c.Value);
            var allClaims = Permissions.GenerateAllPermissions();
            var allPermissions = allClaims.Select(p => new CheckBoxViewModel { DisplayValue = p }).ToList();

            foreach (var permission in allPermissions)
            {
                if (roleClaims.Any(c => c == permission.DisplayValue))
                    permission.IsSelected = true;
            }

            var viewModel = new PermissionsFormViewModel
            {
                RoleId = roleId,
                RoleName = role.Name,
                RoleCalims = allPermissions
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPermissions(PermissionsFormViewModel model)
        {
            if (model.RoleName == null)
            {
                ModelState.AddModelError("RoleName", "Pleaze Enter The Name Of Group");

                return View(model);
            }
            if (await roleManager.RoleExistsAsync(model.RoleName))
            {
                var rol = await roleManager.FindByNameAsync(model.RoleName);
                if (rol.Name == model.RoleName && rol.Id != model.RoleId)
                {
                    ModelState.AddModelError("RoleName", "The Group is exists..! Pleaze Enter Another Name Of Group");

                    var viewModel = Permissions.ReturnPermissionsFormViewModel(model);

                    return View(viewModel);
                }

            }

            if (model.RoleCalims.Where(p => p.IsSelected == true).Count() == 0)
            {
                ModelState.AddModelError("RoleName", "Please specify group permissions before adding");
                return View(model);
            }

            var role = await roleManager.FindByIdAsync(model.RoleId);

            if (role == null)
                return NotFound();

            var roleClaims = await roleManager.GetClaimsAsync(role);
            role.Name = model.RoleName;
            foreach (var claim in roleClaims)
                await roleManager.RemoveClaimAsync(role, claim);

            var selectedClaims = model.RoleCalims.Where(c => c.IsSelected).ToList();

            foreach (var claim in selectedClaims)
                await roleManager.AddClaimAsync(role, new Claim("Permission", claim.DisplayValue));

            return RedirectToAction(nameof(Index));
        }

        [Authorize("Permissions.Group.Delete")]
        public async Task<IActionResult> DeletePermissions(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            

            if (role == null)
                return NotFound();
            var roleClaims = roleManager.GetClaimsAsync(role).Result.Select(c => c.Value).ToList();
            var allClaims = Permissions.GenerateAllPermissions();
            var allPermissions = allClaims.Select(p => new CheckBoxViewModel { DisplayValue = p }).ToList();

            foreach (var permission in allPermissions)
            {
                if (roleClaims.Any(c => c == permission.DisplayValue))
                    permission.IsSelected = true;
            }
            var viewModel = new PermissionsFormViewModel
            {
                RoleId = roleId,
                RoleName = role.Name,
                RoleCalims = allPermissions
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePermissionsPost(string Id)
        {
            try
            {
                var role = await roleManager.FindByIdAsync(Id);
                var allUserInRole = await db.UserRoles.Where(x => x.RoleId == role.Id).Select(x => x.UserId).Distinct().ToListAsync();

                foreach (var item in allUserInRole)
                {
                    var user = await userManager.FindByIdAsync(item);
                    var x = await userManager.DeleteAsync(user);

                }

                var result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {

                throw;
            }
          
        }

    }
}
