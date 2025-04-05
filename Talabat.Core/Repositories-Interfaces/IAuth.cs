using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Services.Auth
{
    public interface IAuth
    {
        Task<string> GenerateTokenAsync(AppUser user, UserManager<AppUser> userManager);
    }
}
