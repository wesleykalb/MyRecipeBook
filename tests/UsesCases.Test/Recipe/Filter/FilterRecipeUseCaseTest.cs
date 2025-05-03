using CommomTesteUtilities.Entities;
using CommomTesteUtilities.Mapper;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using MyRecipeBook.Tests.CommomTesteUtilities.Repositories;
using MyRecipeBook.Tests.CommomTesteUtilities.Requests;
using Shouldly;
using UsesCases.Test.User.Profile;

namespace UsesCases.Test.Recipe.Filter;

public class FilterRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();

        var request = RequestFilterRecipeJsonBuilder.Build();

        var recipes = RecipeBuilder.Collection(user);

        var useCase = CreateUseCase(user, recipes);

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Recipes.ShouldNotBeNull();
        result.Recipes.Count.ShouldBe(recipes.Count);
    }

    [Fact]
    public async Task Error_Cooking_Time_Invalid()
    {
        var (user, _) = UserBuilder.Build();

        var request = RequestFilterRecipeJsonBuilder.Build();
        request.CookingTimes.Add((MyRecipeBook.Communication.Enums.CookingTime)9999);

        var recipes = RecipeBuilder.Collection(user);

        var useCase = CreateUseCase(user, recipes);

        Func<Task> act = async () => {await useCase.Execute(request);};

        (await act.ShouldThrowAsync<ErrorOnValidationException>())
            .ShouldSatisfyAllConditions(
                () => act.ShouldThrow<ErrorOnValidationException>().ErrorMessages.Contains(ResourceMessagesException.COOKING_TIME_NOT_SUPPORTED),
                () => act.ShouldThrow<ErrorOnValidationException>().ErrorMessages.Count.ShouldBe(1)
            );
    }
    
    private static FilterRecipeUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.User user,
        IList<MyRecipeBook.Domain.Entities.Recipe> recipes)
        {
            var loggedUser = LoggedUserBuilder.Build(user);
            var mapper = MapperBuilder.Build();
            var recipeRepository = new RecipeReadOnlyRepositoryBuilder()
                .Filter(user, recipes)
                .Build();

            return new FilterRecipeUseCase(loggedUser, mapper,recipeRepository);
        }
}