using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Entities.Enums;
using MyRecipeBook.Domain.Services.OpenAI;
using OpenAI;
using OpenAI.Chat;

namespace MyRecipeBook.Infraestructure.Services.OpenAI;

public class ChatGPTService : IGenerateRecipeAI
{
    private readonly ChatClient _chatClient;

    public ChatGPTService(ChatClient client)
    {
        _chatClient = client;
    }

    public async Task<GenerateRecipeDto> Generate(IList<string> ingredients)
    {
        var messages = new List<ChatMessage>()
        {
            new SystemChatMessage(ResourceOpenAI.STARTING_GENERATE_RECIPE),
            new UserChatMessage(string.Join(";", ingredients))
        };

        var response = await _chatClient.CompleteChatAsync(messages);

        var responseRecipe = response.Value.Content[0].Text;

        var responseList = responseRecipe.Split("\n")
            .Where(response => !string.IsNullOrWhiteSpace(response))
            .Select(response => response.Replace("[", "").Replace("]", ""))
            .ToList();

        var step = 1;
        return new GenerateRecipeDto()
        {
           Title = responseList[0],
           CookingTime = (CookingTime)Enum.Parse(typeof(CookingTime), responseList[1]),
           Ingredients = [.. responseList[2].Split(";")],
           Instructions = [.. responseList[3].Split("@").Select(instruction => new GeneratedInstructionDto()
           {
                Text = instruction.Trim(),
                Step = step++
           })]
        };
    }
}