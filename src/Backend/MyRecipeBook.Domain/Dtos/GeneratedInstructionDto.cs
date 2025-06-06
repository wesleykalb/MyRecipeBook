namespace MyRecipeBook.Domain.Dtos;

public record GeneratedInstructionDto
{
    public int Step { get; set; }
    public string Text { get; set; } = string.Empty;
}