using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.ServiceBus;
using MyRecipeBook.Infraestructure.DataAccess.Repositories;

namespace MyRecipeBook.Application.UseCases.User.Delete.Request;

public class RequestDeleteUserUseCase : IRequestDeleteUserUseCase
{
    private readonly IDeleteUserQueue _deleteUserQueue;
    private readonly IUserUpdateOnlyRepository _userUpdateOnlyRepository;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork; 
    public RequestDeleteUserUseCase(
        IDeleteUserQueue deleteUserQueue,
        IUserUpdateOnlyRepository userUpdateOnlyRepository,
        ILoggedUser loggedUser,
        IUnitOfWork unitOfWork
    )
    {
        _deleteUserQueue = deleteUserQueue;
        _userUpdateOnlyRepository = userUpdateOnlyRepository;
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute()
    {
        var loggedUser = await _loggedUser.User();

        var user = await _userUpdateOnlyRepository.GetById(loggedUser.id);

        user.Active = false;
        _userUpdateOnlyRepository.Update(user);

        await _unitOfWork.Commit();
        
        await _deleteUserQueue.SendMessage(loggedUser);
    }
}