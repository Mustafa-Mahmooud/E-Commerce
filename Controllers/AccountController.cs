using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.api.Attributes;
using Talabat.APIs.DTOS;
using Talabat.Core.Entities;
using Talabat.Services.Auth;

namespace Talabat.APIs.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuth _auth;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IAuth auth)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _auth = auth;
        }

        // Modified Login endpoint
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            // Check if the user exists based on the provided email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("No such account was found.");
            }

            // Verify the password using the sign-in manager
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return BadRequest("Invalid email or password.");
            }

            // Generate JWT token and return user details along with token
            return Ok(await GenerateUserDto(user));
        }

        // Modified Register endpoint
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            // Check if the email is already registered
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest("Email is already registered.");
            }

            // Create a new AppUser entity based on the provided data
            var user = new AppUser
            {
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DisplayName = model.DisplayName,
                UserName = !string.IsNullOrEmpty(model.Email) ? model.Email.Split("@")[0] : null
            };

            // Create the user in the Identity system
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Generate JWT token and return user details along with token
            return Ok(await GenerateUserDto(user));
        }

        // Modified GetUser endpoint (Authorized)
        [CacheAttributes(30)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            // Retrieve the email from the JWT token claims
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("User is not authenticated.");
            }

            // Retrieve the user based on the email from the JWT token
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Generate JWT token and return user details along with token
            return Ok(await GenerateUserDto(user));
        }

        // Helper method to generate UserDto with token
        private async Task<UserDto> GenerateUserDto(AppUser user)
        {
            var token = await _auth.GenerateTokenAsync(user, _userManager);
            return new UserDto
            {
                Email = user.Email,
                DispalyName = user.DisplayName,
                Token = token // Attach the token to the response DTO
            };
        }
    }
}
