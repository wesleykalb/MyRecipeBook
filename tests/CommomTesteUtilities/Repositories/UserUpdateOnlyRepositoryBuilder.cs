using Moq;
using MyRecipeBook.Domain.Repositories.User;

namespace MyRecipeBook.tests.CommomTesteUtilities.Repositories;

public class UserUpdateOnlyRepositoryBuilder
{
    private readonly Mock<IUserUpdateOnlyRepository> _repository;

    public UserUpdateOnlyRepositoryBuilder() => _repository = new Mock<IUserUpdateOnlyRepository>();
    
    public UserUpdateOnlyRepositoryBuilder GetById(MyRecipeBook.Domain.Entities.User user)
    {
        _repository.Setup(x => x.GetById(user.id)).ReturnsAsync(user);
        return this;
    }

    public IUserUpdateOnlyRepository Build() => _repository.Object;
}