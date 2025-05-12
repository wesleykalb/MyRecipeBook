
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using MyRecipeBook.Infraestructure.DataAccess.Repositories;

namespace MyRecipeBook.Application.UseCases.Recipe.Delete;

public class DeleteRecipeUseCase : IDeleteRecipeUseCase
{
    private readonly IRecipeReadOnlyRepository _recipeReadOnlyRepository;
    private readonly IRecipeWriteOnlyRepository _recipeWriteOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;
    private readonly IBlobStorageService _blobStorageService;
    public DeleteRecipeUseCase(
        IRecipeReadOnlyRepository recipeReadOnlyRepository,
        IRecipeWriteOnlyRepository recipeWriteOnlyRepository,
        IUnitOfWork unitOfWork,
        ILoggedUser loggedUser,
        IBlobStorageService blobStorageService)
    {
        _recipeReadOnlyRepository = recipeReadOnlyRepository;
        _recipeWriteOnlyRepository = recipeWriteOnlyRepository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
        _blobStorageService = blobStorageService;
    }
    public async Task Execute(long recipeId)
    {
        var loggedUser = await _loggedUser.User();

        var recipe = await _recipeReadOnlyRepository.GetRecipeById(loggedUser, recipeId) ?? throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);
        
        if (!string.IsNullOrEmpty(recipe.ImageIdentifier))
        {
            await _blobStorageService.Delete(loggedUser, recipe.ImageIdentifier);
        }

        await _recipeWriteOnlyRepository.Delete(recipeId);

        await _unitOfWork.Commit();
    }
}