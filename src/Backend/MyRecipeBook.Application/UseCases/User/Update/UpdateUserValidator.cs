using Azure.Core;
using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.User.Update;

public class UpdateUserValidator : AbstractValidator<RequestUpdateUserJson>
{
    public UpdateUserValidator()
    {
        RuleFor(request => request.Name).NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleFor(request => request.Email).NotEmpty().WithMessage(ResourceMessagesException.EMAIL_EMPTY);

        When (request => !string.IsNullOrEmpty(request.Email), () =>
        {
            RuleFor(request => request.Email)
                .EmailAddress()
                .WithMessage(ResourceMessagesException.INVALID_EMAIL);
        });
    }
}