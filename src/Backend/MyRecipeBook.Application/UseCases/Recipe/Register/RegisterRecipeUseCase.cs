using AutoMapper;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Security.LoggedUser;
using MyRecipeBook.Exceptions.ExceptionsBase;
using MyRecipeBook.Infraestructure.DataAccess.Repositories;

namespace MyRecipeBook.Application.UseCases.Recipe;

public class RegisterRecipeUseCase : IRegisterRecipeUseCase
{
    private readonly IRecipeWriteOnlyRepository _recipeWriteOnlyRepository;
    private readonly ILoggedUser _loggedUser;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterRecipeUseCase(
        IRecipeWriteOnlyRepository recipeWriteOnlyRepository,
        ILoggedUser loggedUser,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _recipeWriteOnlyRepository = recipeWriteOnlyRepository;
        _loggedUser = loggedUser;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisteredRecipeJson> Execute(RequestRecipeJson request)
    {
        Validate(request);

        var user = await _loggedUser.User();

        var recipe = _mapper.Map<Domain.Entities.Recipe>(request);
        recipe.UserId = user.id;

        var instructions = request.Instructions.OrderBy(i => i.Step).ToList();
        for (int i = 0; i < instructions.Count; i++)
        {
            instructions[i].Step = i + 1;
        }
        recipe.Instructions = _mapper.Map<IList<Domain.Entities.Instruction>>(instructions);

        await _recipeWriteOnlyRepository.Add(recipe);

        await _unitOfWork.Commit();
        
        return _mapper.Map<ResponseRegisteredRecipeJson>(recipe);
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