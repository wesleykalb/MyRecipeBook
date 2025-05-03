
using FluentMigrator.Infrastructure;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.LoggedUser;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using MyRecipeBook.Infraestructure.DataAccess.Repositories;

namespace MyRecipeBook.Application.UseCases.User.Update;

public class ChangePasswordUseCase : IChangePasswordUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserUpdateOnlyRepository _userUpdateOnlyRepository;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IPasswordEncripter _passwordEncripter;

    public ChangePasswordUseCase(
        ILoggedUser loggedUser,
        IUserUpdateOnlyRepository userUpdateOnlyRepository,
        IPasswordEncripter passwordEncripter,
        IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _userUpdateOnlyRepository = userUpdateOnlyRepository;
        _passwordEncripter  = passwordEncripter;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(RequestChangePasswordJson request)
    {
        var loggedUser = await _loggedUser.User();

        Validate(request, loggedUser);

        var user = await _userUpdateOnlyRepository.GetById(loggedUser.id);

        user.Password = _passwordEncripter.Encrypt(request.NewPassword);

        _userUpdateOnlyRepository.Update(user);

        await _unitOfWork.Commit();
    }

    private void Validate(RequestChangePasswordJson request, Domain.Entities.User loggedUser)
    {
        var result = new ChangePasswordValidator().Validate(request);

        var currentPasswordEncripted = _passwordEncripter.Encrypt(request.Password);

        if (!currentPasswordEncripted.Equals(loggedUser.Password))
        {
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, ResourceMessagesException.PASSWORD_INVALID_ERROR));
        }

        if (!result.IsValid)
        {
            throw new ErrorOnValidationException([.. result.Errors.Select(e => e.ErrorMessage)]);
        }
    }
}