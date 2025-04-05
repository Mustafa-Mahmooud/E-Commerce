using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Repository.Data.Identity
{
    public static class AppUserDataSeeding
    {

        public static async Task SeedAsync(UserManager<AppUser> userManager)
        {
            if (userManager.Users.Count() == 0)
            {
                var user = new AppUser()
                {
                    DisplayName = "Mustafa Mahmoud",
                    UserName = "Mustafa",
                    Email = "Mustafa@Gmail.com",
                    PhoneNumber = "01206550157",
                    
                    
                   
                };
                await userManager.CreateAsync(user,"#Mustafam7m0d");
            }
        }
    } 
}