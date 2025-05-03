using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Domain.Security.Tokens;

namespace MyRecipeBook.Infraestructure.Security.Tokens.Access.Generator
{
    public class JwtTokenGenerator : JwtTokenHandler, IAccessTokenGenerator
    {
        private readonly uint _expirationTimeInMinutes;
        private readonly string _signingKey;

        public JwtTokenGenerator(string signingKey, uint expirationTimeInMinutes)
        {
            _signingKey = signingKey;
            _expirationTimeInMinutes = expirationTimeInMinutes;
        }
        public string Generate(Guid userIdentifier)
        {
            var claims = new List<Claim>()
            {   
                new(ClaimTypes.Sid, userIdentifier.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_expirationTimeInMinutes),
                SigningCredentials = new SigningCredentials(SecurityKey(_signingKey), SecurityAlgorithms.HmacSha256Signature),
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }

    }
}
