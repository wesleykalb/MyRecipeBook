using CommomTesteUtilities.Entities;
using CommomTesteUtilities.Mapper;
using MyRecipeBook.Application.UseCases.Dashboard;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Tests.CommomTesteUtilities.Repositories;
using Shouldly;
using UsesCases.Test.User.Profile;

namespace UsesCases.Test.Recipe.Dashboard;

public class GetDashboardUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipes = RecipeBuilder.Collection(user);
        var useCase = CreateUseCase(user, recipes);

        var result = await useCase.Execute();

        result.ShouldNotBeNull();
        result.Recipes.Count.ShouldBe(recipes.Count);
    }

    private static GetDashboardUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.User user ,
        IList<MyRecipeBook.Domain.Entities.Recipe> recipes)
    {
        var recipeReadOnlyRepository = new RecipeReadOnlyRepositoryBuilder()
            .GetForDashboard(user, recipes)
            .Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var mapper = MapperBuilder.Build();

        return new GetDashboardUseCase(recipeReadOnlyRepository, loggedUser, mapper);
    }
}