using System.Reflection.Metadata.Ecma335;
using MyRecipeBook.Domain.Security.Tokens;

namespace MyRecipeBook.Infraestructure.Security.Tokens.Access.Generator;

public class AccessTokenGenerator : IAccessTokenGenerator
{
    public string Generate(Guid userIdentifier) => Convert.ToBase64String(Guid.NewGuid().ToByteArray());
}