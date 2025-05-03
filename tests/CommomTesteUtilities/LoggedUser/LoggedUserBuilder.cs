using Moq;
using MyRecipeBook.Domain.Security.LoggedUser;

namespace UsesCases.Test.User.Profile;

public class LoggedUserBuilder
{
    public static ILoggedUser Build(MyRecipeBook.Domain.Entities.User user)
    {
        var mock = new Mock<ILoggedUser>();

        mock.Setup(x => x.User()).ReturnsAsync(user);

        return mock.Object;        
    }
}