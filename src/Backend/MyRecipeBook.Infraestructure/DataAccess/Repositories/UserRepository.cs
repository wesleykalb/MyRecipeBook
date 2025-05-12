using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.User;

namespace MyRecipeBook.Infraestructure.DataAccess.Repositories
{
    public class UserRepository : IUserReadOnlyRepository, IUserWriteOnlyRepository, IUserUpdateOnlyRepository, IUserDeleteOnlyRepository
    {
        private readonly MyRecipeBookDbContext _context;

        public UserRepository(MyRecipeBookDbContext context) => _context = context;
        
        public async Task Add(User user) => await _context.Users.AddAsync(user);

        public async Task<bool> ExistActiveUserWithEmail(string email) => await _context.Users.AnyAsync(x => x.Email.Equals(email) && x.Active);

        public async Task<bool> ExistsActiveUserWithIdentifier(Guid userIdentifier) => await _context.Users.AnyAsync(x => x.UserIdentifier.Equals(userIdentifier) && x.Active);

        public async Task<User?> GetByEmailAndPassword(string email, string password)
        {
            return await _context
                .Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email.Equals(email) && x.Password.Equals(password));
        }

        public async Task<User> GetById(long id)
        {
            return await _context
                .Users
                .FirstAsync(x => x.id.Equals(id));
        }

        public void Update(User user) => _context.Users.Update(user);

        public async Task DeleteAccount(Guid userIdentifier)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == userIdentifier);

            if (user is null)
                return;

            var recipes = _context.Recipes.Where(x => x.UserId == user.id);

            _context.Recipes.RemoveRange(recipes);

            _context.Users.Remove(user);
        }
    }
}
