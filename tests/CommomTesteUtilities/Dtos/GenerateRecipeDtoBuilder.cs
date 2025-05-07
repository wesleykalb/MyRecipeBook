using Bogus;
using MyRecipeBook.Domain.Entities.Enums;
using MyRecipeBook.Domain.Dtos;

namespace CommomTesteUtilities.Dtos;

public class GenerateRecipeDtoBuilder
{
    public static GenerateRecipeDto Build()
    {
        return new Faker<GenerateRecipeDto>()
            .RuleFor(r => r.Title, f => f.Lorem.Word())
            .RuleFor(r => r.CookingTime, f => f.PickRandom<CookingTime>())
            .RuleFor(r => r.Ingredients, f => f.Make(1, () => f.Commerce.ProductName()))
            .RuleFor(r => r.Instructions, f => f.Make(1, () => new GeneratedInstructionDto()
            {
                Text = f.Lorem.Paragraph(),
                Step = 1 
            }));
    }
}