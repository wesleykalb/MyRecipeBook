using Moq;
using MyRecipeBook.Domain.Repositories.User;

namespace CommomTesteUtilities.Repositories;

public class UserDeleteOnlyRepositoryBuilder
{
    private readonly Mock<IUserDeleteOnlyRepository> _repository;

    public UserDeleteOnlyRepositoryBuilder() => _repository = new Mock<IUserDeleteOnlyRepository>();

    public IUserDeleteOnlyRepository Build() => _repository.Object;
}