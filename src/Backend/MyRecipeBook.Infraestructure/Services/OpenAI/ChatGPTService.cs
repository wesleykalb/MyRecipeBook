using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Services.OpenAI;

namespace MyRecipeBook.Infraestructure.Services.OpenAI;

public class ChatGPTService : IGenerateRecipeAI
{
    private readonly string CHAT_MODEL = "gpt-4o";

    public Task<GenerateRecipeDto> Generate(IList<string> ingredients)
    {
        throw new NotImplementedException();
    }
}