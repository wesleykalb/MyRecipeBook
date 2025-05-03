using Bogus;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Entities.Enums;

namespace CommomTesteUtilities.Entities;

public class RecipeBuilder
{
    public static IList<Recipe> Collection(User user, uint count = 2)
    {
        var list = new List<Recipe>();

        if (count == 0)
            count = 1;
        
        var recipeId = 1;
        for (var i = 0; i < count; i++)
        {
            var recipe = Build(user);
            recipe.id = recipeId ++;
            list.Add(recipe);
        }
        return list;
    }

    public static Recipe Build(User user)
    {
        return new Faker<Recipe>()
            .RuleFor(r => r.id, () => 1)
            .RuleFor(r => r.Title, f => f.Lorem.Word())
            .RuleFor(r => r.CookingTime, f => f.PickRandom<CookingTime>())
            .RuleFor(r => r.Difficulty, f => f.PickRandom<Difficulty>())
            .RuleFor(r => r.Ingredients, f => f.Make(1, () => new Ingredient
            {
                id = 1,
                Item = f.Commerce.ProductName(),
            }))
            .RuleFor(r => r.Instructions, f => f.Make(1, () => new Instruction
            {
                id = 1,
                Step = 1,
                Text = f.Lorem.Paragraph()
            }))
            .RuleFor(r => r.DishTypes, f => f.Make(1, () => new MyRecipeBook.Domain.Entities.DishType
            {
                id = 1,
                Type = f.PickRandom<MyRecipeBook.Domain.Entities.Enums.DishType>()
            }))
            .RuleFor(r => r.UserId, _ => user.id);
    }
}