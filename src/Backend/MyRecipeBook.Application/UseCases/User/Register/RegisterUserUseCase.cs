
using AutoMapper;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Token;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using MyRecipeBook.Infraestructure.DataAccess.Repositories;

namespace MyRecipeBook.Application.UseCases.User.Register
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private readonly IUserReadOnlyRepository _readOnlyRepository;
        private readonly IUserWriteOnlyRepository _writeOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordEncripter _passwordEncripter;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly ITokenRepository _tokenRepository;

        public RegisterUserUseCase(
            IUserReadOnlyRepository userRepository,
            IUserWriteOnlyRepository userWriteOnlyRepository,
            IMapper mapper,
            IPasswordEncripter passwordEncripter,
            IUnitOfWork unitOfWork,
            IAccessTokenGenerator accessTokenGenerator,
            IRefreshTokenGenerator refreshTokenGenerator,
            ITokenRepository tokenRepository)
        {
            _readOnlyRepository = userRepository;
            _writeOnlyRepository = userWriteOnlyRepository;
            _mapper = mapper;
            _passwordEncripter = passwordEncripter;
            _unitOfWork = unitOfWork;
            _accessTokenGenerator = accessTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _tokenRepository = tokenRepository;
        }
        public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson content)
        {
            await Validate(content);

            var user = _mapper.Map<Domain.Entities.User>(content);
            user.Password = _passwordEncripter.Encrypt(content.Password);
            user.UserIdentifier = Guid.NewGuid();

            await _writeOnlyRepository.Add(user);
            await _unitOfWork.Commit();

            var refreshToken = await CreateAndSaveRefreshToken(user);

            return new ResponseRegisteredUserJson
            {
                Name = user.Name,
                Tokens = new ResponseTokensJson
                {
                    AccessToken = _accessTokenGenerator.Generate(user.UserIdentifier),
                    RefreshToken = refreshToken
                }
            };
        }

        private async Task Validate(RequestRegisterUserJson content)
        { 
            var validator = new RegisterUserValidator();

            var result = validator.Validate(content);

            var userExist = await _readOnlyRepository.ExistActiveUserWithEmail(content.Email);

            if (userExist)
                result.Errors.Add(new FluentValidation.Results.ValidationFailure("Email", ResourceMessagesException.EMAIL_ALREADY_REGISTERED));

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }

        private async Task<string> CreateAndSaveRefreshToken(Domain.Entities.User user)
        {
            var refreshToken = new RefreshToken{
                Value = _refreshTokenGenerator.Generate(),
                UserId = user.id
            };

            await _tokenRepository.SaveNewRefreshToken(refreshToken);

            await _unitOfWork.Commit();

            return refreshToken.Value;
        }
    }
}
