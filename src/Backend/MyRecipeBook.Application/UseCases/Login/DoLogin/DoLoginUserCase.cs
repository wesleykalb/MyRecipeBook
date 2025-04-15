using MyRecipeBook.Application.Services.Cryptography;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Login.DoLogin
{
    public class DoLoginUserCase : IDoLoginUserCase
    {
        private readonly IUserReadOnlyRepository _userReadOnlyRepository;
        private readonly PasswordEncripter _passwordEncripter;
        private readonly IAccessTokenGenerator _accessTokenGenerator;

        public DoLoginUserCase(
            IUserReadOnlyRepository userReadOnlyRepository,
            PasswordEncripter passwordEncripter,
            IAccessTokenGenerator accessTokenGenerator)
        {
            _userReadOnlyRepository = userReadOnlyRepository;
            _passwordEncripter = passwordEncripter;
            _accessTokenGenerator = accessTokenGenerator;
        }
        public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request)
        {
            var password = _passwordEncripter.Encript(request.Password);

            var user = await _userReadOnlyRepository.GetByEmailAndPassword(request.Email, password) ?? throw new InvalidLoginException();

            return new ResponseRegisteredUserJson
            { 
                Name = user.Name,
                Tokens = new ResponseTokensJson
                {
                    AccessToken = _accessTokenGenerator.Generate(user.UserIdentifier),
                }
            };
        }
    }
}
