using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infraestructure.Security.Tokens.Refresh;

namespace CommonTestUtilities.Tokens;
public class RefreshTokenGeneratorBuilder
{
    public static IRefreshTokenGenerator Build() => new RefreshTokenGenerator();
}