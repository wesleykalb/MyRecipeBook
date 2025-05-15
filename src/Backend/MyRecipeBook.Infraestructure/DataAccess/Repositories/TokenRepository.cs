using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Token;

namespace MyRecipeBook.Infraestructure.DataAccess.Repositories;

public class TokenRepository : ITokenRepository
{
    private MyRecipeBookDbContext _dbContext;

    public TokenRepository(MyRecipeBookDbContext dbContext) => _dbContext = dbContext;
    public async Task<RefreshToken?> Get(string refreshToken)
    {
        return await _dbContext
            .RefreshTokens
            .AsNoTracking()
            .Include(token => token.User)
            .FirstOrDefaultAsync(token => token.Value.Equals(refreshToken));
    }

    public async Task SaveNewRefreshToken(RefreshToken refreshToken)
    {
        var tokens = _dbContext.RefreshTokens.Where(x => x.UserId == refreshToken.UserId);

        _dbContext.RefreshTokens.RemoveRange(tokens);

        await _dbContext.RefreshTokens.AddAsync(refreshToken);
    }
}