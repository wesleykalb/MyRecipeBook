using FileTypeChecker.Extensions;
using FileTypeChecker.Types;
using Microsoft.AspNetCore.Http;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using MyRecipeBook.Infraestructure.DataAccess.Repositories;

namespace MyRecipeBook.Application.UseCases.Recipe.Image;

public class AddUpdateImageCoverUseCase : IAddUpdateImageCoverUseCase
{
    private readonly IRecipeUpdateOnlyRepository _recipeUpdateOnlyRepository;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;

    public AddUpdateImageCoverUseCase(
        IRecipeUpdateOnlyRepository recipeUpdateOnlyRepository,
        ILoggedUser loggedUser,
        IUnitOfWork unitOfWork)
    {
        _recipeUpdateOnlyRepository = recipeUpdateOnlyRepository;
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
    }
    public async Task Execute(long recipeId, IFormFile file)
    {
        var loggedUser = await _loggedUser.User();

        var recipe = await _recipeUpdateOnlyRepository.GetById(loggedUser, recipeId) 
            ?? throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);

        var fileStream = file.OpenReadStream();
        if (!fileStream.Is<JointPhotographicExpertsGroup>() && !fileStream.Is<PortableNetworkGraphic>())
        {
            throw new ErrorOnValidationException([ResourceMessagesException.ONLY_IMAGES_ACCEPTED]);
        }
    }
}