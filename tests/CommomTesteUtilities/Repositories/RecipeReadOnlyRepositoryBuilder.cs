using Moq;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace MyRecipeBook.Tests.CommomTesteUtilities.Repositories;

public class RecipeReadOnlyRepositoryBuilder
{
    private readonly Mock<IRecipeReadOnlyRepository> _repository;

    public RecipeReadOnlyRepositoryBuilder() => _repository = new Mock<IRecipeReadOnlyRepository>();

    public RecipeReadOnlyRepositoryBuilder Filter(User user, IList<Recipe> recipes)
    {
        _repository.Setup(x => x.Filter(user, It.IsAny<FilterRecipesDto>()))
            .ReturnsAsync(recipes);
        return this;
    }

    public RecipeReadOnlyRepositoryBuilder GetRecipeById(User user, Recipe? recipe)
    {
        if (recipe is not null)
            _repository.Setup(x => x.GetRecipeById(user, recipe.id)).ReturnsAsync(recipe);

        return this;
    }

    public RecipeReadOnlyRepositoryBuilder GetForDashboard(User user, IList<Recipe> recipes)
    {
        _repository.Setup(x => x.GetForDashboard(user))
            .ReturnsAsync(recipes);
        return this;
    }

    public IRecipeReadOnlyRepository Build() => _repository.Object;
}