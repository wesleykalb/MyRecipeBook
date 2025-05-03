using MyRecipeBook.Domain.Dtos;

namespace MyRecipeBook.Domain.Repositories.Recipe;

public interface IRecipeReadOnlyRepository
{
    public Task<IList<Entities.Recipe>> Filter(Entities.User user, FilterRecipesDto filters);
    public Task<Entities.Recipe?> GetRecipeById(Entities.User user, long recipeId);
    public Task<IList<Entities.Recipe>> GetForDashboard(Entities.User user);
}