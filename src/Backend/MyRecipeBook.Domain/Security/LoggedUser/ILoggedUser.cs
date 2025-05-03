using MyRecipeBook.Domain.Entities;

namespace MyRecipeBook.Domain.Security.LoggedUser;
public interface ILoggedUser
{
    public Task<User> User();
}   