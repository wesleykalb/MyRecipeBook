using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MyRecipeBook.Infraestructure.Security.Tokens.Access;
public abstract class JwtTokenHandler
{
    protected static SymmetricSecurityKey SecurityKey(string signingKey)
    {
        var bites = Encoding.UTF8.GetBytes(signingKey);

        return new SymmetricSecurityKey(bites);
    }   
}