using MyRecipeBook.Domain.Entities.Enums;

namespace MyRecipeBook.Domain.Dtos;

public record FilterRecipesDto
{
    public string? RecipeTitle_Ingredient { get; init; }
    public IList<Difficulty> Difficulties { get; init; } = [];
    public IList<DishType> DishTypes { get; init; } = [];
    public IList<CookingTime> CookingTimes { get; init; } = [];
}