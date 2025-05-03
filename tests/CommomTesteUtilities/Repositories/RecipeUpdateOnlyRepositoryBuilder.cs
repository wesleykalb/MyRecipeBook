using Moq;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace CommonTestUtilities.Repositories;

public class RecipeUpdateOnlyRepositoryBuilder
{
    private readonly Mock<IRecipeUpdateOnlyRepository> _repository;

    public RecipeUpdateOnlyRepositoryBuilder() => _repository = new Mock<IRecipeUpdateOnlyRepository>();

    public RecipeUpdateOnlyRepositoryBuilder GetById(MyRecipeBook.Domain.Entities.User user, MyRecipeBook.Domain.Entities.Recipe? recipe)
    {
        if (recipe is not null)
        {
            _repository.Setup(x => x.GetById(user, recipe.id)).ReturnsAsync(recipe);
        }
        return this;
    }

    public IRecipeUpdateOnlyRepository Build() => _repository.Object;
}
