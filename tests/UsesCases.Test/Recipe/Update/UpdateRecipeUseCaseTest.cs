using System.Threading.Tasks;
using CommomTesteUtilities.Entities;
using CommomTesteUtilities.Mapper;
using CommomTesteUtilities.Repositories;
using CommomTesteUtilities.Requests;
using CommonTestUtilities.Repositories;
using Microsoft.EntityFrameworkCore.Query;
using MyRecipeBook.Application.UseCases.Recipe.Update;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using UsesCases.Test.User.Profile;

namespace UsesCases.Test.Recipe.Update;

public class UpdateRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestRecipeJsonBuilder.Build();
        
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);

        Func<Task> act = async () => {await useCase.Execute(recipe.id, request);};
        
        await act.ShouldNotThrowAsync();
    }

     [Fact]
    public async Task Recipe_NotFound()
    {
        var (user, _) = UserBuilder.Build();

        var request = RequestRecipeJsonBuilder.Build();

        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, null);

        Func<Task> act = async () => {await useCase.Execute(recipeId: 1000, request);};

        (await act.ShouldThrowAsync<NotFoundException>())
            .ShouldSatisfyAllConditions(
                () => act.ShouldThrow<NotFoundException>().Message.Contains(ResourceMessagesException.RECIPE_NOT_FOUND)
            );
    }

    [Fact]
    public async Task Error_Title_Empty()
    {
        var (user, _) = UserBuilder.Build();

        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;

        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);

        Func<Task> act = async () => {await useCase.Execute(recipe.id, request);};

        (await act.ShouldThrowAsync<ErrorOnValidationException>())
            .ShouldSatisfyAllConditions(
                () => act.ShouldThrow<ErrorOnValidationException>().Message.Contains(ResourceMessagesException.RECIPE_TITLE_EMPTY)
            );
    }

    private static UpdateRecipeUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.User user,
        MyRecipeBook.Domain.Entities.Recipe? recipe)
    {
        var recipeUpdateOnlyRepository = new RecipeUpdateOnlyRepositoryBuilder().GetById(user, recipe).Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var unitOfWork = UnitOfWorkBuilder.Build();
        var mapper = MapperBuilder.Build();

        return new UpdateRecipeUseCase(recipeUpdateOnlyRepository, loggedUser, unitOfWork, mapper);
    }
}