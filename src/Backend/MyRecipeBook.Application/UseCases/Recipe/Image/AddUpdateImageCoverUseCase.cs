using FileTypeChecker.Extensions;
using FileTypeChecker.Types;
using Microsoft.AspNetCore.Http;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using MyRecipeBook.Infraestructure.DataAccess.Repositories;

namespace MyRecipeBook.Application.UseCases.Recipe.Image;

public class AddUpdateImageCoverUseCase : IAddUpdateImageCoverUseCase
{
    private readonly IRecipeUpdateOnlyRepository _recipeUpdateOnlyRepository;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;

    public AddUpdateImageCoverUseCase(
        IRecipeUpdateOnlyRepository recipeUpdateOnlyRepository,
        ILoggedUser loggedUser,
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorageService)
    {
        _recipeUpdateOnlyRepository = recipeUpdateOnlyRepository;
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
        _blobStorageService = blobStorageService;
    }
    public async Task Execute(long recipeId, IFormFile file)
    {
        var loggedUser = await _loggedUser.User();

        var recipe = await _recipeUpdateOnlyRepository.GetById(loggedUser, recipeId) 
            ?? throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);

        var fileStream = file.OpenReadStream();

        var (IsValidImage, extension) = StreamImageExtensions.ValidateAndGetImageExtension(fileStream);
        
        if (!IsValidImage)
        {
            throw new ErrorOnValidationException([ResourceMessagesException.ONLY_IMAGES_ACCEPTED]);
        }

        if (string.IsNullOrEmpty(recipe.ImageIdentifier))
        {
            recipe.ImageIdentifier = $"{Guid.NewGuid()}{extension}";

            _recipeUpdateOnlyRepository.Update(recipe);

            await _unitOfWork.Commit();
        }

        await _blobStorageService.Upload(loggedUser, fileStream, recipe.ImageIdentifier);
    }
}