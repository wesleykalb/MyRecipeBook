using CommomTesteUtilities.Requests;
using Shouldly;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exceptions;

namespace Validator.Test.User.Register
{
    public class RegisterUserValidatorTest
    {
        [Fact]
        public void Success()
        {
            var validator = new RegisterUserValidator();

            var request = RequestRegisterUserJsonBuilder.Build();

            var result = validator.Validate(request);

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Error_Name_Empty()
        {
            var validator = new RegisterUserValidator();

            var request = RequestRegisterUserJsonBuilder.Build();
            request.Name = string.Empty;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions(
                () => result.Errors.ShouldBeUnique(),
                () => result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.NAME_EMPTY)
            );
        }

        [Fact]
        public void Error_Email_Empty()
        {
            var validator = new RegisterUserValidator();

            var request = RequestRegisterUserJsonBuilder.Build();
            request.Email = string.Empty;

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions(
                () => result.Errors.ShouldHaveSingleItem(),
                () => result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.EMAIL_EMPTY)
            );
        }

        [Fact]
        public void Error_Email_Invalid()
        {
            var validator = new RegisterUserValidator();

            var request = RequestRegisterUserJsonBuilder.Build();
            request.Email = "email.com";

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions(
                () => result.Errors.ShouldHaveSingleItem(),
                () => result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.INVALID_EMAIL)
            );
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Error_Password_Invalid(int passwordLenght)
        {
            var validator = new RegisterUserValidator();

            var request = RequestRegisterUserJsonBuilder.Build();

            var result = validator.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldSatisfyAllConditions(
                () => result.Errors.ShouldHaveSingleItem(),
                () => result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.INVALID_PASSWORD)
            );
        }
    }
}
