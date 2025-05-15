using CommomTesteUtilities.Cryptography;
using CommomTesteUtilities.Entities;
using CommomTesteUtilities.Repositories;
using CommomTesteUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using MyRecipeBook.tests.CommomTesteUtilities.Repositories;
using Shouldly;
using UsesCases.Test.User.Profile;

namespace UsesCases.Test.User.ChangePassword;

public class ChangePasswordUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        (var user, var password) = UserBuilder.Build();

        var request = RequestChangePasswordJsonBuilder.Build();
        request.Password = password;

        var useCase = CreateUserCase(user);

        Func<Task> act = async () => {await useCase.Execute(request);};

        await act.ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Error_New_Password_Empty()
    {
        (var user, var password) = UserBuilder.Build();

        var request = new RequestChangePasswordJson
        {
            Password = password,
            NewPassword = string.Empty
        };

        var useCase = CreateUserCase(user);

        Func<Task> act = async () => {await useCase.Execute(request);};

        (await act.ShouldThrowAsync<ErrorOnValidationException>())
            .ShouldSatisfyAllConditions(
                () => act.ShouldNotBeNull(),
                () => act.ShouldThrow<ErrorOnValidationException>().ErrorMessages.Contains(ResourceMessagesException.PASSWORD_EMPTY)
            );
    }

    [Fact]
    public async Task Error_Current_Password_Different()
    {
        (var user, var password) = UserBuilder.Build();

        var request = RequestChangePasswordJsonBuilder.Build();

        var useCase = CreateUserCase(user);

        Func<Task> act = async () => {await useCase.Execute(request);};

        (await act.ShouldThrowAsync<ErrorOnValidationException>())
            .ShouldSatisfyAllConditions(
                () => act.ShouldNotBeNull(),
                () => act.ShouldThrow<ErrorOnValidationException>().ErrorMessages.Contains(ResourceMessagesException.PASSWORD_INVALID_ERROR)
            );
    }

    private static ChangePasswordUseCase CreateUserCase(MyRecipeBook.Domain.Entities.User user)
    {
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var userUpdateOnlyRepository = new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();
        var passwordEncripter = PasswordEncripterbBuilder.Build();

        return new ChangePasswordUseCase(
            loggedUser,
            userUpdateOnlyRepository,
            passwordEncripter,
            unitOfWork);
    }
}