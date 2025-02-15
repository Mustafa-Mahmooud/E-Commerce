using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using Talabat.APIs.Helper;
using Talabat.Core.Entities;
using Talabat.Core.Repositories_Interfaces;
using Talabat.Repository.Data;
using Talabat.Repository.Data.DataSeed;
using Talabat.Repository.Data.Identity;
using Talabat.Repository.Repositories;
using Talabat.Services.Auth;
using Talabat.Services.Caching;
using static OpenAI.GPT3.ObjectModels.SharedModels.IOpenAiModels;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            #region Dependency Injection 
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient<ChatGptService>();

            // AppStore Database
            builder.Services.AddDbContext<StoreContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Users Database (Identity)
            builder.Services.AddDbContext<AppIdentityContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDbContext")));

            // Basket Database (Caching)
            builder.Services.AddSingleton<IConnectionMultiplexer>((serverprovider) =>
            {
                var connection = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connection);
            });

            //Identity
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
               

            }).AddEntityFrameworkStores<AppIdentityContext>();

            //ChatGptAPi 
            builder.Services.AddControllers();
            builder.Services.AddHttpClient<ChatGptService>();

            //Register Services
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddScoped(typeof(IAuth), typeof(TokenService));
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped(typeof(IBasketCustomer), typeof(BasketCustomerRepository));
            builder.Services.AddScoped<IPayment, Payment>();
            builder.Services.AddSingleton<ICache, Cache>();

            builder.Services.AddAuthentication().AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
                };


            });


            var app = builder.Build();

            #endregion

            #region Database Migration and Seeding
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var _dbContext = services.GetRequiredService<StoreContext>();
            var _usermanager = services.GetRequiredService<UserManager<AppUser>>();

            var _identityDbContext = services.GetRequiredService<AppIdentityContext>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                await _dbContext.Database.MigrateAsync();
                await StoreContextSeed.SeedAsync(_dbContext);
                await _identityDbContext.Database.MigrateAsync();
                await AppUserDataSeeding.SeedAsync(_usermanager);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred during migration or seeding.");
            }
            #endregion

            #region Middleware Configuration
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.MapControllers();
            #endregion

            app.Run();
        }
    }
}
