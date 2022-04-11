using BAL.Contants;
using DLL.Data;
using DLL.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BAL.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedHRrAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManger)
        {
            var defaultUser = new AppUser
            {
                UserName = "Server",
                FullName = "Server",
                Email = "Server@domain.com",
                EmailConfirmed = true,
                IsActive = true
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "Server@123");
                await userManager.AddToRoleAsync(defaultUser, Roles.Server.ToString());
            }
            await roleManger.SeeDLLlClaimsForHR();
        }

        public static async Task SeedAdminUserAsync(UserManager<AppUser> userManager)
        {
            var defaultUser = new AppUser
            {
                UserName = "Admin",
                FullName = "Admin",
                Email = "Admin@domain.com",
                EmailConfirmed = true,
                IsActive = true
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "Admin@123");
                await userManager.AddToRoleAsync(defaultUser,Roles.Admin.ToString() );
            }
            
        }

        private static async Task SeeDLLlClaimsForHR(this RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(Roles.Server.ToString());
            await roleManager.AdDLLlPermissionClaims(adminRole);
        }
        public static async Task AdDLLlPermissionClaims(this RoleManager<IdentityRole> roleManager, IdentityRole role)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GenerateAllPermissions();

            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(c => c.Type == "Permission" && c.Value == permission))
                    await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }
        }
        
    }
}