using Moq;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace CommomTesteUtilities.Repositories;

public class RecipeWriteOnlyRepositoryBuilder
{
    public static IRecipeWriteOnlyRepository Build()
    {
        var mock = new Mock<IRecipeWriteOnlyRepository>();
        return mock.Object;
    }
}