using BookStoreApi.Static;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Data
{
    public static class SeedData
    {
        //private readonly static string _usersAdminName = "admin";
        private readonly static string _usersAdminEmail = "admin@bookstore.com";
        private readonly static string _usersAdminPassword = "P@ssword1";
        //private readonly static string _usersCustomer1Name = "JoeJackson";
        private readonly static string _usersCustomer1Email = "joeJackson@gmail.com";
        //private readonly static string _usersCustomer2Name = "JillJohnson";
        //private readonly static string _usersCustomer2Email = "jillJohnson@yahoo.com";
        private readonly static string _usersCustomerPassword = "P@ssword1";


        public async static Task Seed(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager);
        }

        private async static Task SeedUsers(UserManager<IdentityUser> userManager)
        {
            if(await userManager.FindByEmailAsync(_usersAdminEmail) == null)
            {
                var user = new IdentityUser { UserName = _usersAdminEmail, Email = _usersAdminEmail };
                var result = await userManager.CreateAsync(user, _usersAdminPassword);
                if(result.Succeeded)
                {
                    await userManager.AddToRolesAsync(user, new List<string>() { Roles.Administrator, Roles.Customer });
                }
            }

            if (await userManager.FindByEmailAsync(_usersCustomer1Email) == null)
            {
                var user = new IdentityUser { UserName = _usersCustomer1Email, Email = _usersCustomer1Email };
                var result = await userManager.CreateAsync(user, _usersCustomerPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, Roles.Customer);
                }
            }

            //if (await userManager.FindByEmailAsync(_usersCustomer2Email) == null)
            //{
            //    var user = new IdentityUser { UserName = _usersCustomer2Email, Email = _usersCustomer2Email };
            //    var result = await userManager.CreateAsync(user, _usersCustomerPassword);
            //    if (result.Succeeded)
            //    {
            //        await userManager.AddToRoleAsync(user, Roles.Customer);
            //    }
            //}

        }

        private async static Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if(!await roleManager.RoleExistsAsync(Roles.Administrator))
            {
                var role = new IdentityRole { Name = Roles.Administrator };
                await roleManager.CreateAsync(role);
            }

            if (!await roleManager.RoleExistsAsync(Roles.Customer))
            {
                var role = new IdentityRole { Name = Roles.Customer };
                await roleManager.CreateAsync(role);
            }
        }
    }
}
