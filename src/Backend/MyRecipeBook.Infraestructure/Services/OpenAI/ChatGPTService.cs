using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Services.OpenAI;
using OpenAI;

namespace MyRecipeBook.Infraestructure.Services.OpenAI;

public class ChatGPTService : IGenerateRecipeAI
{
    private readonly string CHAT_MODEL = "gpt-4o";
    private readonly OpenAIClient _openAIAPI;

    public ChatGPTService(OpenAIClient client)
    {
        _openAIAPI = client;
    }

    public async Task<GenerateRecipeDto> Generate(IList<string> ingredients)
    {
        var chat = _openAIAPI.GetChatClient(CHAT_MODEL);
        var response = chat.CompleteChat("diga, isso é um teste");
        var result = response.Value.Content[0].Text;
        var recipe = new GenerateRecipeDto
        {
            Title = result! // Assuming GenerateRecipeDto has a property named RecipeContent
        };

        return recipe;
    }
}