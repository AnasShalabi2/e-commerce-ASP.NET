using KASHOP.DAL.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KASHOP.PL.Utils
{
    public class RoleSeedData : ISeedData
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleSeedData(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task DataSeed()
        {
            string[] roles = { "superAdmin", "admin", "user" };
            if (!await _roleManager.Roles.AnyAsync())
            {
                foreach (var role in roles)
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                        throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                }

            }
        }
    }
}

