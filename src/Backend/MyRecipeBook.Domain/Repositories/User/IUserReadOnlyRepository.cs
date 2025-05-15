namespace MyRecipeBook.Domain.Repositories.User
{
    public interface IUserReadOnlyRepository
    {
        public Task<bool> ExistActiveUserWithEmail(string email);

        public Task<bool> ExistsActiveUserWithIdentifier(Guid userIdentifier);

        public Task<Entities.User?> GetByEmail(string email);
    }
}
