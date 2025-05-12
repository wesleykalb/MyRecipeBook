using AutoMapper;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;

namespace MyRecipeBook.Application.UseCases.Dashboard;

public class GetDashboardUseCase : IGetDashboardUseCase
{
    private readonly IRecipeReadOnlyRepository _recipeReadOnlyRepository;
    private readonly ILoggedUser _loggedUser;
    private readonly IMapper _mapper;
    private readonly IBlobStorageService _blobStorageService;
    public GetDashboardUseCase(
        IRecipeReadOnlyRepository recipeReadOnlyRepository,
        ILoggedUser loggedUser,
        IMapper mapper,
        IBlobStorageService blobStorageService)
    {
        _recipeReadOnlyRepository = recipeReadOnlyRepository;
        _loggedUser = loggedUser;
        _mapper = mapper;
        _blobStorageService = blobStorageService;
    }
    public async Task<ResponseRecipesJson> Execute()
    {
        var loggedUser = await _loggedUser.User();

        var recipes = await _recipeReadOnlyRepository.GetForDashboard(loggedUser);

        return new ResponseRecipesJson()
        {
            Recipes = await RecipeListExtension.MapToShortRecipeJson(recipes, loggedUser, _blobStorageService, _mapper)
        };
    }
}