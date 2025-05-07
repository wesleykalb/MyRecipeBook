using CommomTesteUtilities.Dtos;
using CommomTesteUtilities.OpenAI;
using CommomTesteUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe.Generate;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UsesCases.Test.Recipe.Generate;

public class GenerateRecipeUseCaseTest
{
    public async Task Success()
    {
        var recipeDto = GenerateRecipeDtoBuilder.Build();

        var request = RequestGenerateRecipeJsonBuilder.Build();

        var useCase = CreateUseCase(recipeDto);

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Title.ShouldBe(recipeDto.Title);
        result.CookingTime.ShouldBe((CookingTime)recipeDto.CookingTime);
        result.Ingredients.ShouldBe(recipeDto.Ingredients);
        result.Difficulty.ShouldBe(Difficulty.Low);
    }

    public async Task Error_Duplicated_Ingredient()
    {
        var recipeDto = GenerateRecipeDtoBuilder.Build();

        var request = RequestGenerateRecipeJsonBuilder.Build(5);
        request.Ingredients[0] = request.Ingredients[1];

        var useCase = CreateUseCase(recipeDto);

        Func<Task> act = async () => { await useCase.Execute(request); };

        (await act.ShouldThrowAsync<ErrorOnValidationException>())
            .ShouldSatisfyAllConditions(
                () => act.ShouldThrow<ErrorOnValidationException>().Message.Contains(ResourceMessagesException.DUPLICATED_INGREDIENTS_IN_LIST)
            );
    }
    private static GenerateRecipeUseCase CreateUseCase(GenerateRecipeDto recipeDto)
    {
        var generateRecipeAI = GenerateRecipeAIBuilder.Build(recipeDto);

        return new GenerateRecipeUseCase(generateRecipeAI);
    }
}