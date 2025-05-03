using System.Threading.Tasks;
using CommomTesteUtilities.Entities;
using CommomTesteUtilities.Mapper;
using CommomTesteUtilities.Repositories;
using CommomTesteUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using UsesCases.Test.User.Profile;

namespace Tests.UsesCases.Test.Recipe.Register;

public class RegisterRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var request = RequestRecipeJsonBuilder.Build();

        (var user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var response = await useCase.Execute(request);

        response.ShouldNotBeNull();
        response.Id.ShouldNotBeNull();
        response.Title.ShouldBe(request.Title);
    }

    [Fact]
    public async Task Error_Title_Empty()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;

        (var user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        Func<Task> act = async () => await useCase.Execute(request);

        (await act.ShouldThrowAsync<ErrorOnValidationException>())
            .ShouldSatisfyAllConditions(
                () => act.ShouldThrow<ErrorOnValidationException>().ErrorMessages.Contains(ResourceMessagesException.RECIPE_TITLE_EMPTY),
                () => act.ShouldThrow<ErrorOnValidationException>().ErrorMessages.Count.ShouldBe(1)
            );
    }

    private static RegisterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
    {
        return new RegisterRecipeUseCase(
            RecipeWriteOnlyRepositoryBuilder.Build(),
            LoggedUserBuilder.Build(user),
            MapperBuilder.Build(),
            UnitOfWorkBuilder.Build()
        );
    }
}