using CommomTesteUtilities.Entities;
using CommomTesteUtilities.Mapper;
using MyRecipeBook.Application.UseCases.User.Profile;
using Shouldly;

namespace UsesCases.Test.User.Profile;

public class GetUserProfileUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        (var user, var _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var result = await useCase.Execute();

        result.ShouldNotBeNull();
        result.Name.ShouldBe(user.Name);
        result.Email.ShouldBe(user.Email);
    }

    private static GetUserProfileUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
    {
        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);

        return new GetUserProfileUseCase(
            loggedUser,
            mapper
        );
    }
}