using CommomTesteUtilities.Entities;
using CommomTesteUtilities.Mapper;
using MyRecipeBook.Application.UseCases.Recipe.GetById;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using MyRecipeBook.Tests.CommomTesteUtilities.Repositories;
using Shouldly;
using UsesCases.Test.User.Profile;

namespace UsesCases.Test.Recipe.GetById;

public class GetRecipeByIdUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();

        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);

        var response = await useCase.Execute(recipe.id);

        response.ShouldNotBeNull();
        response.Id.ShouldNotBeNullOrWhiteSpace();
        response.Title.ShouldBe(recipe.Title);
    }

    [Fact]
    public async Task RecipeNotFound()
    {
        var (user, _) = UserBuilder.Build();

        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, null);

        Func<Task> act = async () => {await useCase.Execute(recipeId: 1000);};

        (await act.ShouldThrowAsync<NotFoundException>())
            .ShouldSatisfyAllConditions(
                () => act.ShouldThrow<NotFoundException>().Message.Contains(ResourceMessagesException.RECIPE_NOT_FOUND)
            );
    }
    private static GetRecipeByIdUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.User user,
        MyRecipeBook.Domain.Entities.Recipe? recipe)
    {
        var recipeReadOnlyRepository  = new RecipeReadOnlyRepositoryBuilder().GetRecipeById(user, recipe).Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var mapper = MapperBuilder.Build();

        return new GetRecipeByIdUseCase(recipeReadOnlyRepository, loggedUser, mapper);
    }
}