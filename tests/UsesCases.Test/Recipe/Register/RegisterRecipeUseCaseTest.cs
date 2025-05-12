using System.Threading.Tasks;
using CommomTesteUtilities.BlobStorage;
using CommomTesteUtilities.Entities;
using CommomTesteUtilities.Mapper;
using CommomTesteUtilities.Repositories;
using CommomTesteUtilities.Requests;
using Microsoft.AspNetCore.Http;
using MyRecipeBook.Application.UseCases.Recipe;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using UsesCases.Test.Recipe.InlineDatas;
using UsesCases.Test.User.Profile;

namespace Tests.UsesCases.Test.Recipe.Register;

public class RegisterRecipeUseCaseTest
{
    [Fact]
    public async Task SuccessWithoutImage()
    {
        var request = RequestRegisterRecipeFormDataBuilder.Build();

        (var user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var response = await useCase.Execute(request);

        response.ShouldNotBeNull();
        response.Id.ShouldNotBeNull();
        response.Title.ShouldBe(request.Title);
    }

    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task SuccessWithImage(IFormFile file)
    {
        var request = RequestRegisterRecipeFormDataBuilder.Build(file);

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
        var request = RequestRegisterRecipeFormDataBuilder.Build();
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

    [Fact]
    public async Task Error_Invalid_File()
    {
        var fileTxt = FormFileBuilder.Txt();

        var request = RequestRegisterRecipeFormDataBuilder.Build(fileTxt);

        (var user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        Func<Task> act = async () => await useCase.Execute(request);

        (await act.ShouldThrowAsync<ErrorOnValidationException>())
            .ShouldSatisfyAllConditions(
                () => act.ShouldThrow<ErrorOnValidationException>().ErrorMessages.Contains(ResourceMessagesException.ONLY_IMAGES_ACCEPTED),
                () => act.ShouldThrow<ErrorOnValidationException>().ErrorMessages.Count.ShouldBe(1)
            );
    }

    private static RegisterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
    {
        return new RegisterRecipeUseCase(
            RecipeWriteOnlyRepositoryBuilder.Build(),
            LoggedUserBuilder.Build(user),
            MapperBuilder.Build(),
            UnitOfWorkBuilder.Build(),
            new BlobStorageServiceBuilder().Build()
        );
    }
}