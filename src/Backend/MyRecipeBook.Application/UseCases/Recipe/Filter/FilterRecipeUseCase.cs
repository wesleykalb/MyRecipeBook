using AutoMapper;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Security.LoggedUser;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter;

public class FilterRecipeUseCase : IFilterRecipeUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IMapper _mapper;
    private readonly IRecipeReadOnlyRepository _recipeReadOnlyRepository;

    public FilterRecipeUseCase(ILoggedUser loggedUser, IMapper mapper, IRecipeReadOnlyRepository recipeReadOnlyRepository)
    {
        _loggedUser = loggedUser;
        _mapper = mapper;
        _recipeReadOnlyRepository = recipeReadOnlyRepository;
    }
    public async Task<ResponseRecipesJson> Execute(RequestFilterRecipeJson request)
    {
        Validate(request);

        var loggedUser = await _loggedUser.User();

        var filter = new FilterRecipesDto()
        {
            RecipeTitle_Ingredient = request.RecipeTitle_Ingredient,
            Difficulties = [.. request.Difficulties.Distinct().Select(c => (Domain.Entities.Enums.Difficulty)c)],
            CookingTimes = [.. request.CookingTimes.Distinct().Select(c => (Domain.Entities.Enums.CookingTime)c)],
            DishTypes = [.. request.DishTypes.Distinct().Select(c => (Domain.Entities.Enums.DishType)c)]
        };

        var recipes = await _recipeReadOnlyRepository.Filter(loggedUser, filter);

        return new ResponseRecipesJson()
        {
            Recipes = _mapper.Map<IList<ResponseShortRecipeJson>>(recipes)
        };
    }

    private static void Validate(RequestFilterRecipeJson request)
    {
        var result = new FilterRecipeValidator().Validate(request);

        if (!result.IsValid)
        {
            throw new ErrorOnValidationException([.. result.Errors.Select(x => x.ErrorMessage).Distinct()]);
        }
    }
}