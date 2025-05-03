using Bogus;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infraestructure.Security.Tokens.Access.Generator;
namespace CommonTestUtilities.Tokens
{
    public class JwtTokenGenerationBuilder
    {
        public static IAccessTokenGenerator Build() => new JwtTokenGenerator(expirationTimeInMinutes: 5, signingKey: "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww");
    }
}