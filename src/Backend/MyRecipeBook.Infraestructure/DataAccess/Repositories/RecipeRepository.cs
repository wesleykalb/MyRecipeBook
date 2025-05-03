using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace MyRecipeBook.Infraestructure.DataAccess.Repositories;

public class RecipeRepositoy : IRecipeWriteOnlyRepository, IRecipeReadOnlyRepository, IRecipeUpdateOnlyRepository
{
    private readonly MyRecipeBookDbContext _context;

    public RecipeRepositoy(MyRecipeBookDbContext context)
    {
        _context = context;
    }

    public async Task Add(Domain.Entities.Recipe recipe) => await _context.Recipes.AddAsync(recipe);

    public async Task Delete(long recipeId)
    {
        var recipe = await _context.Recipes.FindAsync(recipeId);

        _context.Recipes.Remove(recipe!);
    }

    public async Task<IList<Recipe>> Filter(User user, FilterRecipesDto filters)
    {
        var query = _context
            .Recipes
            .Include(x => x.Ingredients)
            .AsNoTracking()
            .Where(x => x.UserId == user.id && x.Active);
        
        if (filters.Difficulties.Any())
        {
            query = query.Where(x => x.Difficulty.HasValue && filters.Difficulties.Contains(x.Difficulty.Value));
        }
        
        if (filters.CookingTimes.Any())
        {
            query = query.Where(x => x.CookingTime.HasValue && filters.CookingTimes.Contains(x.CookingTime.Value));
        }
        
        if (filters.DishTypes.Any())
        {
            query = query.Where(x => x.DishTypes.Any(x => filters.DishTypes.Contains(x.Type)));
        }

        if (filters.RecipeTitle_Ingredient != null)
        {
            query = query.Where(x => x.Title.Contains(filters.RecipeTitle_Ingredient)
            || x.Ingredients.Any(i => i.Item.Contains(filters.RecipeTitle_Ingredient)));
        }
        
        return await query.ToListAsync();
    }

    public async Task<Recipe?> GetById(User user, long recipeId)
    {
        return await GetFullRecipe()
            .FirstOrDefaultAsync(x => x.UserId == user.id && x.Active && x.id == recipeId);
    }

    public async Task<IList<Recipe>> GetForDashboard(User user)
    {
        return await _context
            .Recipes
            .AsNoTracking()
            .Include(x => x.Ingredients)
            .Where(x => x.UserId == user.id && x.Active)
            .OrderByDescending(x => x.CreatedOn)
            .Take(5)
            .ToListAsync();

    }

    public async Task<Recipe?> GetRecipeById(User user, long recipeId)
    {
        return await GetFullRecipe()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == user.id && x.Active && x.id == recipeId);
    }

    public void Update(Recipe recipe) => _context.Recipes.Update(recipe);

    private IIncludableQueryable<Recipe, IList<Instruction>> GetFullRecipe()
    {
        return _context
            .Recipes
            .Include(x => x.Ingredients)
            .Include(x => x.DishTypes)
            .Include(x => x.Instructions);
    }
}