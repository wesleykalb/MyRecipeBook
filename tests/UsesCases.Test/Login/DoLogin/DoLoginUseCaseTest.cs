using CommomTesteUtilities.Cryptography;
using CommomTesteUtilities.Entities;
using CommomTesteUtilities.Repositories;
using CommomTesteUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Application.UseCases.Login.DoLogin;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UsesCases.Test.Login.DoLogin
{
    public class DoLoginUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            (var user, var password) = UserBuilder.Build();

            var useCase = CreateUseCase(user);

            var result = await useCase.Execute(new RequestLoginJson
            {
                Email = user.Email,
                Password = password
            });

            result.ShouldNotBeNull();
            result.Tokens.ShouldNotBeNull();
            result.Name.ShouldNotBeNullOrWhiteSpace();
            result.Name.ShouldBe(user.Name);
            result.Tokens.AccessToken.ShouldNotBeNullOrWhiteSpace();

        }
        [Fact]
        public async Task Error_Invalid_User()
        { 
            var request = RequestLoginJsonBuilder.Build();
            var useCase = CreateUseCase();

            var exception = async () => { await useCase.Execute(request); };
            (await exception.ShouldThrowAsync<InvalidLoginException>()).Message.ShouldBe(ResourceMessagesException.EMAIL_OR_PASSWORD_INVALID);
        }
        private static DoLoginUserCase CreateUseCase(MyRecipeBook.Domain.Entities.User? user = null)
        {
            var userReadOnlyRepository = new UserReadOnlyRepositoryBuilder();
            var passwordEncripter = PasswordEncripterbBuilder.Build();
            var accessTokenGenerator = JwtTokenGenerationBuilder.Build();

            if (user is not null)
                userReadOnlyRepository.GetByEmailAndPassword(user);

            return new DoLoginUserCase(userReadOnlyRepository.Build(), passwordEncripter, accessTokenGenerator);
        }
    }
}
