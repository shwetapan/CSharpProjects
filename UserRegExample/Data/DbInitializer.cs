using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserRegExample.Models;

namespace UserRegExample.Data
{
    public static class DbInitializer
    {
        public static async Task InitilizeAsync(IServiceProvider serviceProvider, UserManager<ApplicationUser> _userManager)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "SuperAdmin", "CustomerCare" };
            IdentityResult result;
            foreach(var roleName in roleNames)
            {
                var roleExists = await RoleManager.RoleExistsAsync(roleName);
                if(!roleExists)
                {
                    result = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            string Email = "superadmin@outlook.com";
            string passwword = "Super@1234";

            if(_userManager.FindByEmailAsync(Email).Result==null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = Email;
                user.Email = Email;
                IdentityResult resultIdentity = _userManager.CreateAsync(user, passwword).Result;
                if(resultIdentity.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "SuperAdmin").Wait();
                }
            }
        }
    }
}
