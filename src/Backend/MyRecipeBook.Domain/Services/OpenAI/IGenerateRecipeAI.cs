using MyRecipeBook.Domain.Dtos;

namespace MyRecipeBook.Domain.Services.OpenAI;

public interface IGenerateRecipeAI
{
    Task<GenerateRecipeDto> Generate(IList<string> ingredients);
}