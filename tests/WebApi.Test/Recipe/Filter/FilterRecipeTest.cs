using System.Globalization;
using System.Net;
using System.Text.Json;
using Azure.Core;
using CommonTestUtilities.Tokens;
using Microsoft.OpenApi.Expressions;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Tests.CommomTesteUtilities.Requests;
using Shouldly;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.Filter;

public class FilterRecipeTest : MyRecipeBookClassFixture
{
    private readonly string METHOD = "recipe/filter";

    private readonly Guid _userIdentifier;

    private string _recipeTitle;
    private MyRecipeBook.Domain.Entities.Enums.Difficulty _difficultyLevel;
    private MyRecipeBook.Domain.Entities.Enums.CookingTime _cookingTime;
    private IList<MyRecipeBook.Domain.Entities.Enums.DishType> _dishTypes;
    public FilterRecipeTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userIdentifier = factory.GetUserIdentifier();
        _recipeTitle = factory.GetRecipeTitle();
        _difficultyLevel = factory.GetDifficulty();
        _cookingTime = factory.GetCookingTime();
        _dishTypes = factory.GetDishTypes();
    }

    [Fact]
    public async Task Success()
    {
        var request = new RequestFilterRecipeJson
        {
            CookingTimes = [(MyRecipeBook.Communication.Enums.CookingTime)_cookingTime],
            Difficulties = [(MyRecipeBook.Communication.Enums.Difficulty)_difficultyLevel],
            DishTypes = _dishTypes.Select(x => (MyRecipeBook.Communication.Enums.DishType)x).ToList(),
            RecipeTitle_Ingredient = _recipeTitle
        };

        var token = JwtTokenGenerationBuilder.Build().Generate(_userIdentifier);

        var response = await DoPost(METHOD, request, token: token);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);
        var recipes = responseData.RootElement.GetProperty("recipes").EnumerateArray();
        recipes.ShouldNotBeEmpty();
    }
    [Fact]
    public async Task NoContent()
    {
        var request = RequestFilterRecipeJsonBuilder.Build();
        request.RecipeTitle_Ingredient = "NonExistentRecipeTitle";

        var token = JwtTokenGenerationBuilder.Build().Generate(_userIdentifier);

        var response = await DoPost(METHOD, request, token: token);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_CookingTime_Invalid(string culture)
    {
        var request = RequestFilterRecipeJsonBuilder.Build();
        request.CookingTimes.Add((MyRecipeBook.Communication.Enums.CookingTime)9999);

        var token = JwtTokenGenerationBuilder.Build().Generate(_userIdentifier);

        var response = await DoPost(METHOD, request, token: token, culture: culture);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        
        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("COOKING_TIME_NOT_SUPPORTED", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(
            () => errors.Count().ShouldBe(1),
            () => errors.First().GetString().ShouldBe(expectedMessage)
        );
    }
}