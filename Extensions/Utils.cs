using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStore.Extensions
{
    public class Utils
    {
        public static string EncodeJWT(
            IConfiguration configuration,
            IEnumerable<Claim> claims,
            DateTime expires,
            string? audience = null)
        {
            var signingKey = Encoding.UTF8.GetBytes(configuration["Authentication:Jwt:SigningSecret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = configuration["Authentication:Jwt:Issuer"],
                Audience = audience,
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                Expires = expires,
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(signingKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(jwtToken);
        }
    }
}
