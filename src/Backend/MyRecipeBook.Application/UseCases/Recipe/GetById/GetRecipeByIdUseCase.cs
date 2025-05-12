using AutoMapper;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.GetById;

public class GetRecipeByIdUseCase : IGetRecipeByIdUseCase
{
    private readonly IMapper _mapper;

    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeReadOnlyRepository _recipeReadOnlyRepository;

    private readonly IBlobStorageService _blobStorageService;

    public GetRecipeByIdUseCase(
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

    public async Task<ResponseRecipeJson> Execute(long recipeId)
    {
        var loggedUser = await _loggedUser.User();

        var recipe = await _recipeReadOnlyRepository.GetRecipeById(loggedUser, recipeId) ?? throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);
        
        var response = _mapper.Map<ResponseRecipeJson>(recipe);

        if (!string.IsNullOrEmpty(recipe.ImageIdentifier))
        {
            var url = await _blobStorageService.GetImageUrl(loggedUser, recipe.ImageIdentifier);
            response.ImageUrl = url;
        }
        return response;
    }
}