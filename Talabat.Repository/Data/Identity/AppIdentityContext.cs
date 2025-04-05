using Microsoft.AspNetCore.Identity.EntityFrameworkCore;  
using Microsoft.EntityFrameworkCore;
using Talabat.Core.Entities;
using Talabat.Core.Entities;




namespace Talabat.Repository.Data.Identity
{
    public class AppIdentityContext : IdentityDbContext<AppUser>  
    {
        public AppIdentityContext(DbContextOptions<AppIdentityContext> options)
            : base(options)
        {
        }

        
    }
}
