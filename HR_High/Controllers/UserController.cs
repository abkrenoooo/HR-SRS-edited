using DLL.Models;
using Microsoft.AspNetCore.Authorization;
using DLL.ViewModel.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Ninject.Activation;

namespace HR_High.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IPasswordHasher<AppUser> passwordHash;

        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, IPasswordHasher<AppUser> passwordHash)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.passwordHash = passwordHash;
        }

        [Authorize("Permissions.User.View")]
        public async Task<IActionResult> Index()
        {
            var users = await userManager.Users
                .Select(user => new UserViewModel { Id = user.Id, UserName = user.UserName,IsActive=user.IsActive, Email = user.Email, Role = userManager.GetRolesAsync(user).Result })
                .ToListAsync();
            if (ViewData["State"] == null)
                ViewData["State"] = "Active";
            return View(users);
        }
        [Authorize("Permissions.User.Create")]
        public async Task<IActionResult> CreateUser()
        {
            var roles = await roleManager.Roles.ToListAsync();
            ViewBag.rolesList = new SelectList(roles, "Id", "Name");
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var roles = await roleManager.Roles.ToListAsync();
                ViewBag.rolesList = new SelectList(roles, "Id", "Name", model.RoleId);
                if (model.RoleId == "Choose Department")
                {
                    ModelState.AddModelError("RoleId", "Pleaze Select The Group ");
                    return View(model);
                }
                var user = await userManager.FindByNameAsync(model.UserName);
                if (user != null && user.UserName == model.UserName && user.Id != model.Id)
                {
                    ModelState.AddModelError("UserName", "The UserName is exists..! Pleaze Enter Another UserName");
                    return View(model);
                }
                var userEmail = await userManager.FindByEmailAsync(model.Email);
                if (userEmail != null && userEmail.Email == model.Email && userEmail.Id != model.Id)
                {
                    ModelState.AddModelError("Email", "The Email is exists..! Pleaze Enter Another Email");
                    return View(model);
                }

                var userVM = new AppUser
                {
                    FullName = model.FullName,
                    UserName = model.UserName,
                    Email = model.Email,
                };
                var result = await userManager.CreateAsync(userVM, model.Password);
                if (result.Succeeded)
                {
                    var userCreated = await userManager.FindByNameAsync(model.UserName);
                    var role = await roleManager.FindByIdAsync(model.RoleId);
                    var userRoles = await userManager.GetUsersInRoleAsync(role.Name);
                    var Role = await roleManager.FindByIdAsync(model.RoleId);
                    await userManager.AddToRoleAsync(userCreated, Role.Name);

                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);

        }
        [Authorize("Permissions.User.Edit")]
        public async Task<IActionResult> EditUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var roles = await roleManager.Roles.ToListAsync();
            var role = roleManager.FindByNameAsync(userManager.GetRolesAsync(user).Result.ElementAt(0)).Result;
            ViewBag.rolesList = new SelectList(roles, "Id", "Name", role.Id);

            var Role = roleManager.FindByNameAsync(userManager.GetRolesAsync(user).Result.ElementAt(0)).Result;

            var viewModel = new EditUser_VM
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                RoleId = Role.Id
        };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUser_VM model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.UserName);
                var roles = await roleManager.Roles.ToListAsync();
                var role = roleManager.FindByNameAsync(userManager.GetRolesAsync(user).Result.ElementAt(0)).Result;
                ViewBag.rolesList = new SelectList(roles, "Id", "Name", role.Id);
                if (model.RoleId == "Choose Department")
                {
                    ModelState.AddModelError("RoleId", "Pleaze Select The Group ");
                    return View(model);
                }

                if (user != null && user.UserName == model.UserName && user.Id != model.Id)
                {
                    ModelState.AddModelError("UserName", "The UserName is exists..! Pleaze Enter Another UserName");
                    return View(model);
                }
                var userEmail = await userManager.FindByEmailAsync(model.Email);
                if (userEmail != null && userEmail.Email == model.Email && userEmail.Id != model.Id)
                {
                    ModelState.AddModelError("Email", "The Email is exists..! Pleaze Enter Another Email");
                    return View(model);
                }
                if (user == null)
                    return NotFound();
                user.FullName = model.FullName;
                user.UserName = model.UserName;
                user.Email = model.Email;
                var result = await userManager.UpdateAsync(user);

                var userId = await userManager.FindByIdAsync(user.Id);

                var userRoles = await userManager.GetRolesAsync(user);

                await userManager.RemoveFromRolesAsync(user, userRoles);
                var Role = await roleManager.FindByIdAsync(model.RoleId);
                await userManager.AddToRoleAsync(user, Role.Name);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        [Authorize("Permissions.User.Delete")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var roles = await roleManager.Roles.ToListAsync();

            var role = roleManager.FindByNameAsync(userManager.GetRolesAsync(user).Result.ElementAt(0)).Result;
            ViewBag.rolesList = new SelectList(roles, "Id", "Name", role.Id);
            var viewModel = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                RoleId = role.Id

            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(UserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
                return NotFound();

            var result = await userManager.DeleteAsync(user);

            var userRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, userRoles);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }
        public async Task<IActionResult> ActiveUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user.IsActive)
            {
                user.IsActive = false;
                await userManager.SetLockoutEnabledAsync(user, false);
                ViewData["State"] = "Active";
            }
            else
            {
                user.IsActive = true;
                await userManager.SetLockoutEnabledAsync(user, true);
                ViewData["State"] = "Not Active";
            }
            if (user == null)
                return NotFound();

            await userManager.UpdateSecurityStampAsync(user);

            var data = ViewData["State"];

            return Json(user.IsActive);
        }
      
    }
}
