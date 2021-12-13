using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookStore.Extensions;
using System.Security.Claims;
using BookStore.Data.Domain;
using BookStore.Models.Dto;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly UserManager<User> userManager;

        public AuthController(IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            UserManager<User> userManager)
        {
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
            this.userManager = userManager;
        }

        #region Access Token

        private LoginResultDto GenerateAccessToken(User user, string? refreshToken)
        {
            var expiryDuration = int.Parse(configuration["Authentication:Jwt:ExpiryDuration"]);
            var expires = DateTime.UtcNow.AddMinutes(expiryDuration);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
            };
            var accessToken = Utils.EncodeJWT(configuration, claims, expires);

            return new LoginResultDto
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expires = new DateTimeOffset(expires).ToUnixTimeSeconds(),
                User = new UserDto
                {
                    Id = user.Id.ToString(),
                    DisplayName = user.DisplayName,
                    AvatarUrl = user.AvatarUrl,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                }
            };
        }

        #endregion

        #region Seed Users

        [HttpPost("seed")]
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Seed()
        {
            if (Request.Host.Host != "localhost")
            {
                return NotFound();
            }

            var user = await userManager.FindByNameAsync("admin");

            if (user == null)
            {
                user = new User()
                {
                    UserName = "admin",
                };

                await userManager.CreateAsync(user, "Admin@123");

                var roleManager = HttpContext.RequestServices.GetRequiredService<RoleManager<Role>>();
                var role = await roleManager.FindByNameAsync(Constants.Role_Administrators);
                if (role == null)
                {
                    role = new Role() { Name = Constants.Role_Administrators };
                    await roleManager.CreateAsync(role);
                }

                await userManager.AddToRoleAsync(user, Constants.Role_Administrators);
            }

            return Ok();
        }

        #endregion
    }
}
