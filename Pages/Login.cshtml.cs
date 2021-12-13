using BookStore.Data.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace BookStore.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<Role> roleManager;

        public LoginModel(SignInManager<User> signInManager, RoleManager<Role> roleManager)
        {
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public IActionResult OnGetAsync(string? returnUrl = null)
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Page("./Login",
                pageHandler: "Callback",
                values: new { returnUrl }),
            };
            return new ChallengeResult("Google", authenticationProperties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(
            string? returnUrl = null, string? remoteError = null)
        {
            // Get the information about the user from the external login provider
            var googleUser = this.User.Identities.FirstOrDefault();
            if (googleUser != null && googleUser.IsAuthenticated)
            {
                var emailAddress = googleUser.FindFirst(ClaimTypes.Email)?.Value;

                if (!string.IsNullOrEmpty(emailAddress))
                {
                    emailAddress = emailAddress.ToLowerInvariant();

                    var user = await signInManager.UserManager.FindByEmailAsync(emailAddress);

                    if (user == null)
                    {
                        // Create new user
                        user = new User
                        {
                            DisplayName = googleUser.Name ?? emailAddress,
                            UserName = emailAddress,
                            NormalizedUserName = emailAddress.ToUpperInvariant(),
                            Email = emailAddress,
                            NormalizedEmail = emailAddress.ToUpperInvariant(),
                            EmailConfirmed = true,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };

                        var nameidentifier = googleUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        user.Logins.Add(new IdentityUserLogin<string>
                        {
                            LoginProvider = googleUser.AuthenticationType,
                            ProviderKey = nameidentifier,
                            ProviderDisplayName = googleUser.AuthenticationType
                        });

                        await signInManager.UserManager.CreateAsync(user);
                    }

                    var identityUser = new ClaimsIdentity(googleUser.AuthenticationType);
                    identityUser.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    identityUser.AddClaim(new Claim(ClaimTypes.Name, user.DisplayName));

                    if (user.Roles != null)
                    {
                        foreach (var roleId in user.Roles)
                        {
                            var role = await roleManager.FindByIdAsync(roleId);
                            if (role != null)
                            {
                                identityUser.AddClaim(new Claim(ClaimTypes.Role, role.Name));
                            }
                        }
                    }

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        RedirectUri = Request.Host.Value
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identityUser), authProperties);
                }
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return LocalRedirect("/");
        }
    }
}
