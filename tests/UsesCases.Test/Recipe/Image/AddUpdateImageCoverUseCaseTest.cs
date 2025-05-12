using System.Threading.Tasks;
using CommomTesteUtilities.BlobStorage;
using CommomTesteUtilities.Entities;
using CommomTesteUtilities.Repositories;
using CommomTesteUtilities.Requests;
using CommonTestUtilities.Repositories;
using Microsoft.AspNetCore.Http;
using MyRecipeBook.Application.UseCases.Recipe.Image;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using UsesCases.Test.Recipe.InlineDatas;
using UsesCases.Test.User.Profile;

namespace UsesCases.Test.Recipe.Image;

public class AddUpdateImageCoverUseCaseTest
{
    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task Success(IFormFile file)
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);
        
        Func<Task> func = async () => await useCase.Execute(recipe.id, file);

        await func.ShouldNotThrowAsync();
    }

    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task RecipeNotFound(IFormFile file)
    {
        var (user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        Func<Task> act = async () => {await useCase.Execute(recipeId: 1000, file);};

        (await act.ShouldThrowAsync<NotFoundException>())
            .ShouldSatisfyAllConditions(
                () => act.ShouldThrow<NotFoundException>().Message.Contains(ResourceMessagesException.RECIPE_NOT_FOUND)
            );
    }

    [Fact]
    public async Task Error_File_Is_Txt()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var file = FormFileBuilder.Txt();

        var useCase = CreateUseCase(user, recipe);
        
        Func<Task> act = async () => {await useCase.Execute(recipeId: 1000, file);};

        (await act.ShouldThrowAsync<NotFoundException>())
            .ShouldSatisfyAllConditions(
                () => act.ShouldThrow<NotFoundException>().Message.Contains(ResourceMessagesException.ONLY_IMAGES_ACCEPTED)
            );
    }

    private static AddUpdateImageCoverUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, MyRecipeBook.Domain.Entities.Recipe? recipe = null)
    {
        var repository = new RecipeUpdateOnlyRepositoryBuilder().GetById(user, recipe).Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var unitOfWork = UnitOfWorkBuilder.Build();
        var blobService = new BlobStorageServiceBuilder().Build();

        return new AddUpdateImageCoverUseCase(repository,
            loggedUser,
            unitOfWork,
            blobService);
    }
}