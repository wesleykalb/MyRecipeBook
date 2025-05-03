
namespace MyRecipeBook.Communication.Responses;

public class ResponseRecipesJson
{
    public IList<ResponseShortRecipeJson> Recipes { get; set; } = [];

    public object Should()
    {
        throw new NotImplementedException();
    }
}