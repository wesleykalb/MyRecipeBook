using Bogus;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;

namespace MyRecipeBook.Tests.CommomTesteUtilities.Requests;

public class RequestFilterRecipeJsonBuilder
{
    public static RequestFilterRecipeJson Build()
    {
        return new Faker<RequestFilterRecipeJson>()
            .RuleFor(x => x.CookingTimes, f => f.Make(1, () => f.PickRandom<CookingTime>()))
            .RuleFor(x => x.Difficulties, f => f.Make(1, () => f.PickRandom<Difficulty>()))
            .RuleFor(x => x.DishTypes, f => f.Make(1, () => f.PickRandom<DishType>()))
            .RuleFor(x => x.RecipeTitle_Ingredient, f => f.Lorem.Word());
    }
}