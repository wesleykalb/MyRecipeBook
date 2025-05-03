using CommomTesteUtilities.Entities;
using CommomTesteUtilities.Mapper;
using CommomTesteUtilities.Repositories;
using MyRecipeBook.Application.UseCases.Recipe.Delete;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using MyRecipeBook.Tests.CommomTesteUtilities.Repositories;
using Shouldly;
using UsesCases.Test.User.Profile;

namespace UsesCases.Test.Recipe.Delete;

public class DeleteRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();

        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);

        Func<Task> act = async () => {await useCase.Execute(recipe.id);};

        await act.ShouldNotThrowAsync();
    }

    [Fact]
    public async Task RecipeNotFound()
    {
        var (user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user, null);

        Func<Task> act = async () => {await useCase.Execute(recipeId: 1000);};

        (await act.ShouldThrowAsync<NotFoundException>())
            .ShouldSatisfyAllConditions(
                () => act.ShouldThrow<NotFoundException>().Message.Contains(ResourceMessagesException.RECIPE_NOT_FOUND)
            );
    }
    private static DeleteRecipeUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.User user,
        MyRecipeBook.Domain.Entities.Recipe? recipe = null)
    {
        var recipeReadOnlyRepository  = new RecipeReadOnlyRepositoryBuilder().GetRecipeById(user, recipe).Build();
        var recipeWriteOnlyRepository = RecipeWriteOnlyRepositoryBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var unitOfWork = UnitOfWorkBuilder.Build();

        return new DeleteRecipeUseCase(recipeReadOnlyRepository, recipeWriteOnlyRepository, unitOfWork, loggedUser);
    }
}