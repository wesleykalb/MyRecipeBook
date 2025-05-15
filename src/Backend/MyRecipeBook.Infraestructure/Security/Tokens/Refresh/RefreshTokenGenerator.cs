using MyRecipeBook.Domain.Security.Tokens;

namespace MyRecipeBook.Infraestructure.Security.Tokens.Refresh;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public string Generate() => Convert.ToBase64String(Guid.NewGuid().ToByteArray());
}