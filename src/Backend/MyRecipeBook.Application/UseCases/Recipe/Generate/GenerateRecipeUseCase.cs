using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Services.OpenAI;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Generate;

public class GenerateRecipeUseCase : IGenerateRecipeUseCase
{
    private readonly IGenerateRecipeAI _generator;

    public GenerateRecipeUseCase(IGenerateRecipeAI generator)
    {
        _generator = generator;
    }
    public async Task<ResponseGeneratedRecipeJson> Execute(RequestGenerateRecipeJson request)
    {
        Valdate(request);

        var response = await _generator.Generate(request.Ingredients);

        return new ResponseGeneratedRecipeJson
        {
            Title = response.Title,
            Ingredients = response.Ingredients,
            CookingTime = (Communication.Enums.CookingTime)response.CookingTime,
            Instructions = response.Instructions.Select(c => new ResponseGeneratedInstructionJson
            {
                Step = c.Step,
                Text = c.Text
            }).ToList(),
            Difficulty = Communication.Enums.Difficulty.Low
        };
    }

    public static void Valdate(RequestGenerateRecipeJson request)
    {
        var result = new GenerateRecipeValidator().Validate(request);
        if (!result.IsValid)
        {
            throw new ErrorOnValidationException([.. result.Errors.Select(x => x.ErrorMessage)]);
        }
    }
}