using AutoMapper;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using MyRecipeBook.Infraestructure.DataAccess.Repositories;

namespace MyRecipeBook.Application.UseCases.Recipe.Update;

public class UpdateRecipeUseCase : IUpdateRecipeUseCase
{
    private readonly IRecipeUpdateOnlyRepository _recipeUpdateOnlyRepository;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateRecipeUseCase(
        IRecipeUpdateOnlyRepository recipeUpdateOnlyRepository,
        ILoggedUser loggedUser,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _recipeUpdateOnlyRepository = recipeUpdateOnlyRepository;
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task Execute(long recipeId, RequestRecipeJson request)
    {
        Validate(request);
        var user = await _loggedUser.User();

        var recipe = await _recipeUpdateOnlyRepository.GetById(user, recipeId) ?? throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);

        recipe.Ingredients.Clear();
        recipe.Instructions.Clear();
        recipe.DishTypes.Clear();

        _mapper.Map(request, recipe);

        var instructions = request.Instructions.OrderBy(i => i.Step).ToList();
        for (int i = 0; i < instructions.Count; i++)
        {
            instructions[i].Step = i + 1;
        }
        recipe.Instructions = _mapper.Map<IList<Domain.Entities.Instruction>>(instructions);

        _recipeUpdateOnlyRepository.Update(recipe);

        await _unitOfWork.Commit();
    }

    private static void Validate(RequestRecipeJson request)
    {
        var result = new RecipeValidator().Validate(request);

        if (!result.IsValid)
        {
            throw new ErrorOnValidationException([.. result.Errors.Select(x => x.ErrorMessage).Distinct()]);
        }
    }
}