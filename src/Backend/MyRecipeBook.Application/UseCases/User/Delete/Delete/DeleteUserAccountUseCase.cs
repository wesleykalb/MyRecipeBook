
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Infraestructure.DataAccess.Repositories;

namespace MyRecipeBook.Application.UseCases.User.Delete.Delete;

public class DeleteUserAccountUseCase : IDeleteUserAccountUseCase
{
    private readonly IUserDeleteOnlyRepository _userDeleteOnlyRepository;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserAccountUseCase(
        IUserDeleteOnlyRepository userDeleteOnlyRepository,
        IBlobStorageService blobStorageService,
        IUnitOfWork unitOfWork
    )
    {
        _userDeleteOnlyRepository = userDeleteOnlyRepository;
        _blobStorageService = blobStorageService;
        _unitOfWork = unitOfWork;
    }
    public async Task Execute(Guid userIdentifier)
    {
        await _blobStorageService.DeleteContainer(userIdentifier);

        await _userDeleteOnlyRepository.DeleteAccount(userIdentifier);

        await _unitOfWork.Commit();
    }
}