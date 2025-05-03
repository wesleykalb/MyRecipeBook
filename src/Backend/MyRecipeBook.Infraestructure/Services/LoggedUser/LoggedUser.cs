using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Security.LoggedUser;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infraestructure.DataAccess;

namespace Namespace.MyRecipeBook.Domain.Security.LoggedUser;
public class LoggedUser : ILoggedUser
{
    private readonly MyRecipeBookDbContext _dbContext;
    private readonly ITokenProvider _tokenProvider;

    public LoggedUser(MyRecipeBookDbContext dbContext, ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
        _dbContext = dbContext;
    }

    public async Task<User> User()
    {
        var token = _tokenProvider.Value();

        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtToken = tokenHandler.ReadJwtToken(token);

        var identifier = jwtToken.Claims.First(x => x.Type == ClaimTypes.Sid).Value;

        var userIdentifier = Guid.Parse(identifier);

        return await _dbContext
            .Users
            .AsNoTracking()
            .FirstAsync(x => x.UserIdentifier == userIdentifier && x.Active);

    }
}
