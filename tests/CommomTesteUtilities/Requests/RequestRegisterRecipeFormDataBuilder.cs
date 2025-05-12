using Bogus;
using Microsoft.AspNetCore.Http;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;

namespace CommomTesteUtilities.Requests;

public class RequestRegisterRecipeFormDataBuilder
{
    public static RequestRegisterRecipeFormData Build(IFormFile? formFile = null)
    {
        var step = 1;
        return new Faker<RequestRegisterRecipeFormData>()
            .RuleFor(x => x.Image, _ => formFile)
            .RuleFor(x => x.Title, f => f.Lorem.Word())
            .RuleFor(x => x.CookingTime, f => f.PickRandom<CookingTime>())
            .RuleFor(x => x.Difficulty, f => f.PickRandom<Difficulty>())
            .RuleFor(x => x.Ingredients, f => f.Make(3, () => f.Commerce.ProductName()))
            .RuleFor(x => x.DishTypes, f => f.Make(3, () => f.PickRandom<DishType>()))
            .RuleFor(x => x.Instructions, f => f.Make(3, () => new RequestInstructionJson
            {
                Step = step++,
                Text = f.Lorem.Paragraph()
            }));
    }
}