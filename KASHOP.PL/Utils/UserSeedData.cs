using KASHOP.DAL.Models;
using Microsoft.AspNetCore.Identity;

namespace KASHOP.PL.Utils
{
    public class UserSeedData : ISeedData
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserSeedData(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task DataSeed()
        {
            await CreateUserAsync("superAdmin@kashop.coom", "superAdmin", "super Admin", "Super@131", "superAdmin");
            await CreateUserAsync("admin@kashop.coom", "admin", "admin", "Admin@131", "admin");
            await CreateUserAsync("user@kashop.coom", "user", "user", "User@131", "user");

        }

        private async Task CreateUserAsync(string email, string userName, string fullName, string password, string role)
        {
            if (await _userManager.FindByEmailAsync(email) != null || await _userManager.FindByNameAsync(userName) != null) return;
            var user = new ApplicationUser
            {
                Email = email,
                UserName = userName,
                FullName = fullName,
                EmailConfirmed = true
            };
               var result = await _userManager.CreateAsync(user,password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            else
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

        }
    }
}
