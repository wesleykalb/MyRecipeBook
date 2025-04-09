
using AutoMapper;
using MyRecipeBook.Application.Services.Cryptography;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.User;
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
        private readonly PasswordEncripter _passwordEncripter;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserUseCase(
            IUserReadOnlyRepository userRepository,
            IUserWriteOnlyRepository userWriteOnlyRepository,
            IMapper mapper,
            PasswordEncripter passwordEncripter,
            IUnitOfWork unitOfWork
            )
        {
            _readOnlyRepository = userRepository;
            _writeOnlyRepository = userWriteOnlyRepository;
            _mapper = mapper;
            _passwordEncripter = passwordEncripter;
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson content)
        {
            await Validate(content);

            var user = _mapper.Map<Domain.Entities.User>(content);
            user.Password = _passwordEncripter.Encript(content.Password);

            await _writeOnlyRepository.Add(user);
            await _unitOfWork.Commit();

            return new ResponseRegisteredUserJson
            {
                Name = user.Name
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

    }
}
