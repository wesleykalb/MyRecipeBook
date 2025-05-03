using Azure.Core;
using CommomTesteUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Exceptions;
using Shouldly;

namespace Validator.Test.Recipe;
public class RecipeValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_Invalid_Cooking_Time()
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.CookingTime = (CookingTime)10;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(
            () => result.Errors.Count.ShouldBe(1),
            () => result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.COOKING_TIME_NOT_SUPPORTED)
        );
    }

    [Fact]
    public void Error_Invalid_Difficulty()
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.Difficulty = (Difficulty)10;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(
            () => result.Errors.Count.ShouldBe(1),
            () => result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.DIFFICULT_LEVEL_NOT_SUPPORTED)
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Error_Empty_Title(string title)
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.Title = title;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(
            () => result.Errors.Count.ShouldBe(1),
            () => result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.RECIPE_TITLE_EMPTY)
        );
    }

    [Fact]
    public void Succes_Cooking_Time_Null()
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.CookingTime = null;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Succes_Difficulty_Null()
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.Difficulty = null;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Success_Dish_Type_Empty()
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.DishTypes.Clear();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_Dish_Type_Invalid()
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.DishTypes.Add((MyRecipeBook.Communication.Enums.DishType)10);

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(
            () => result.Errors.Count.ShouldBe(1),
            () => result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.DISH_TYPE_NOT_SUPPORTED)
        );
    }

    [Fact]
    public void Error_Ingredient_Empty()
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.Ingredients.Clear();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(
            () => result.Errors.Count.ShouldBe(1),
            () => result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.AT_LEAST_ONE_INGREDIENT)
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Error_Ingredient_Empty_Value(string ingredient)
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.Ingredients.Add(ingredient);

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(
            () => result.Errors.Count.ShouldBe(1),
            () => result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.INGREDIENT_EMPTY)
        );
    }

    [Fact]
    public void Error_Same_Step_Instructions()
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions.First().Step = request.Instructions.Last().Step;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(
            () => result.Errors.Count.ShouldBe(1),
            () => result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.TWO_OR_MORE_INSTRUCTIONS_SAME_ORDER)
        );
    }

    [Fact]
    public void Error_Negative_Step_Instructions()
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions.First().Step = -1;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(
            () => result.Errors.Count.ShouldBe(1),
            () => result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.NON_NEGATIVE_INSTRUCTION_STEP)
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Error_Instruction_Empty_Value(string text)
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions.First().Text = text;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(
            () => result.Errors.Count.ShouldBe(1),
            () => result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.INSTRUCTION_EMPTY)
        );
    }

    [Fact]
    public void Error_Instructions_Too_Long()
    {
        var validator = new RecipeValidator();

        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions.First().Text = RequestStringGenerator.Paragraphs(minCharaters: 2001);

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(
            () => result.Errors.Count.ShouldBe(1),
            () => result.Errors[0].ErrorMessage.ShouldBe(ResourceMessagesException.INSTRUCTION_EXCEEDS_LIMIT_CHARACTERS)
        );
    }
}