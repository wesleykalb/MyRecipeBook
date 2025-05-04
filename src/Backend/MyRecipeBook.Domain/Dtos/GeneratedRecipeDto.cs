using MyRecipeBook.Domain.Entities.Enums;

namespace MyRecipeBook.Domain.Dtos;

public record GenerateRecipeDto
{
    public string Title { get; set; } = string.Empty;
    public IList<string> Ingredients { get; set; } = [];
    public IList<GeneratedInstructionDto> Instructions { get; set; } = [];
    public CookingTime CookingTime { get; set; }
}