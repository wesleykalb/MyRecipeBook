using AutoMapper;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter;

public class FilterRecipeUseCase : IFilterRecipeUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IMapper _mapper;
    private readonly IRecipeReadOnlyRepository _recipeReadOnlyRepository;
    private readonly IBlobStorageService _blobStorageService;

    public FilterRecipeUseCase(
        ILoggedUser loggedUser,
        IMapper mapper,
        IRecipeReadOnlyRepository recipeReadOnlyRepository,
        IBlobStorageService blobStorageService)
    {
        _loggedUser = loggedUser;
        _mapper = mapper;
        _recipeReadOnlyRepository = recipeReadOnlyRepository;
        _blobStorageService = blobStorageService;
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
            Recipes = await RecipeListExtension.MapToShortRecipeJson(recipes,loggedUser, _blobStorageService, _mapper)
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