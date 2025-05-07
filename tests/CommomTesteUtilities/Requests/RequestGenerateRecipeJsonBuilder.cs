using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommomTesteUtilities.Requests;

public class RequestGenerateRecipeJsonBuilder
{
    public static RequestGenerateRecipeJson Build(int count = 5)
    {
        return new Faker<RequestGenerateRecipeJson>()
            .RuleFor(x => x.Ingredients, f => f.Make(count, () => f.Commerce.ProductName()));
    }
}