using System.Threading.Tasks;
using CommomTesteUtilities.Entities;
using CommomTesteUtilities.Repositories;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using MyRecipeBook.tests.CommomTesteUtilities.Repositories;
using MyRecipeBook.tests.CommomTesteUtilities.Requests;
using Shouldly;
using UsesCases.Test.User.Profile;

namespace UsesCases.Test.User.Update;

public class UpdateUserUseCaseTest
{
    [Fact]
    public void Success()
    {
        (var user, var _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();

        var useCase = CreateUserCase(user);

        Func<Task> act = async () => {await useCase.Execute(request);};

        act.ShouldNotThrow();

        user.Email.ShouldBe(request.Email);
        user.Name.ShouldBe(request.Name);
    }

    [Fact]
    public async Task Error_Name_Empty()
    {
        (var user, var _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = string.Empty;

        var useCase = CreateUserCase(user);

        Func<Task> act = async () => {await useCase.Execute(request);};

        (await act.ShouldThrowAsync<ErrorOnValidationException>())
            .ShouldSatisfyAllConditions(
                () => act.ShouldThrow<ErrorOnValidationException>().ErrorMessages.Count.ShouldBe(1),
                () => act.ShouldThrow<ErrorOnValidationException>().ErrorMessages.Contains(ResourceMessagesException.NAME_EMPTY)
            );

        user.Email.ShouldNotBe(request.Email);
        user.Name.ShouldNotBe(request.Name);
    }

    [Fact]
    public async Task Error_Email_Already_Registered()
    {
        (var user, var _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();

        var useCase = CreateUserCase(user, request.Email);

        Func<Task> act = async () => {await useCase.Execute(request);};

        (await act.ShouldThrowAsync<ErrorOnValidationException>())
            .ShouldSatisfyAllConditions(
                () => act.ShouldThrow<ErrorOnValidationException>().ErrorMessages.Count.ShouldBe(1),
                () => act.ShouldThrow<ErrorOnValidationException>().ErrorMessages.Contains(ResourceMessagesException.NAME_EMPTY)
            );

        user.Email.ShouldNotBe(request.Email);
        user.Name.ShouldNotBe(request.Name);
    }

    private static UpdateUserUseCase CreateUserCase(MyRecipeBook.Domain.Entities.User user, string? email = null)
    {
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var userUpdateOnlyRepository = new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();

        var userReadOnlyRepository = new UserReadOnlyRepositoryBuilder();

        if (!string.IsNullOrWhiteSpace(email))
        {
            userReadOnlyRepository.Exists_Active_User_With_Email(email!);
        }

        return new UpdateUserUseCase(
            loggedUser,
            userUpdateOnlyRepository,
            userReadOnlyRepository.Build(),
            unitOfWork);
    }
}