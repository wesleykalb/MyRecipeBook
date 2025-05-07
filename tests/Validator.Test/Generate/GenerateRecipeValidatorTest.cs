using CommomTesteUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe.Generate;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Exceptions;
using Shouldly;

namespace Validator.Test.Generate;

public class GenerateRecipeValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new GenerateRecipeValidator();

        var request = RequestGenerateRecipeJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_More_Maximum_Ingredient()
    {
        var validator = new GenerateRecipeValidator();

        var request = RequestGenerateRecipeJsonBuilder.Build(MyRecipeBookRuleConstants.MAXIMUM_INGREDIENTS_GENERATE_RECIPE + 1);

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(
            () => result.Errors.Count.ShouldBe(1),
            () => result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.INVALID_NUMBER_INGREDIENTS)
        );
    }
    
     [Fact]
    public void Error_Duplicated_Ingredient()
    {
        var validator = new GenerateRecipeValidator();

        var request = RequestGenerateRecipeJsonBuilder.Build(5);
        request.Ingredients[0] = request.Ingredients[1];

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(
            () => result.Errors.Count.ShouldBe(1),
            () => result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.DUPLICATED_INGREDIENTS_IN_LIST)
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Error_Empty_Ingredient(string ingredient)
    {
        var validator = new GenerateRecipeValidator();

        var request = RequestGenerateRecipeJsonBuilder.Build(1);
        request.Ingredients.Add(ingredient);

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(
            () => result.Errors.Count.ShouldBe(1),
            () => result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.INGREDIENT_EMPTY)
        );
    }

    [Fact]
    public void Error_Ingredient_Not_Following_Pattern()
    {
        var validator = new GenerateRecipeValidator();

        var request = RequestGenerateRecipeJsonBuilder.Build(4);
        request.Ingredients[0] = " //";

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(
            () => result.Errors.Count.ShouldBe(1),
            () => result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.INGREDIENT_NOT_FOLLOWING_PATTERN)
        );
    }
}