using FluentValidation;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.Recipe.Generate;

public class GenerateRecipeValidator : AbstractValidator<RequestGenerateRecipeJson>
{
    public GenerateRecipeValidator()
    {
        var maxIngredients = MyRecipeBookRuleConstants.MAXIMUM_INGREDIENTS_GENERATE_RECIPE;
        RuleFor(x => x.Ingredients.Count).InclusiveBetween(1, maxIngredients).WithMessage(ResourceMessagesException.INVALID_NUMBER_INGREDIENTS);
        RuleFor(x => x.Ingredients).Must(x => x.Count == x.Distinct().Count()).WithMessage(ResourceMessagesException.DUPLICATED_INGREDIENTS_IN_LIST);
        RuleFor(x => x.Ingredients).ForEach(rule => 
        {
            rule.Custom((value, context) =>
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    context.AddFailure("Ingredients", ResourceMessagesException.INGREDIENT_EMPTY);
                    return;
                }
                if (value.Count(c => c == ' ') > 3 || value.Count(c => c == '/') > 1)
                {
                    context.AddFailure("Ingredients", ResourceMessagesException.INGREDIENT_NOT_FOLLOWING_PATTERN);
                    return;
                }
            });
        });

    }
}