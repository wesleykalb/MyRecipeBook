using CommomTesteUtilities.Cryptography;
using CommomTesteUtilities.Mapper;
using CommomTesteUtilities.Repositories;
using CommomTesteUtilities.Requests;
using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using Xunit.Sdk;

namespace UsesCases.Test.User.Register
{
    public class RegisterUserUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            var request = RequestRegisterUserJsonBuilder.Build();

            var result = await CreateUserCase().Execute(request);

            result.ShouldNotBeNull();
            result.Name.ShouldBe(request.Name);
        }

        [Fact]
        public async Task Error_Email_Alread_Registered()
        {
            var request = RequestRegisterUserJsonBuilder.Build();
            var useCase = CreateUserCase(request.Email);

            Func<Task> act = async () => await useCase.Execute(request);
            (await act.ShouldThrowAsync<ErrorOnValidationException>())
                .ShouldSatisfyAllConditions(
                    () => act.ShouldNotBeNull(),
                    () => act.ShouldThrow<ErrorOnValidationException>().ErrorMessages.Contains(ResourceMessagesException.EMAIL_ALREADY_REGISTERED)
                );
        }

        [Fact]
        public async Task Error_Name_Empty()
        {
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Name = string.Empty;
            var useCase = CreateUserCase();

            Func<Task> act = async () => await useCase.Execute(request);
            (await act.ShouldThrowAsync<ErrorOnValidationException>())
                .ShouldSatisfyAllConditions(
                    () => act.ShouldNotBeNull(),
                    () => act.ShouldThrow<ErrorOnValidationException>().ErrorMessages.Contains(ResourceMessagesException.NAME_EMPTY)
                );
        }

        private static RegisterUserUseCase CreateUserCase(string? email = null)
        {
            var mapper = MapperBuilder.Build();

            var passwordEncripter = PasswordEncripterbBuilder.Build();

            var writeRepository = UserWriteOnlyRepositoryBuilder.Build();

            var readRepositoryBuilder = new UserReadOnlyRepositoryBuilder();

            var unitOfWork = UnitOfWorkBuilder.Build();

            if (email.IsNullOrEmpty() == false)
            {
                readRepositoryBuilder.Exists_Active_User_With_Email(email!);
            }

            return new RegisterUserUseCase(readRepositoryBuilder.Build(), writeRepository, mapper, passwordEncripter, unitOfWork);
        }
    }
}
