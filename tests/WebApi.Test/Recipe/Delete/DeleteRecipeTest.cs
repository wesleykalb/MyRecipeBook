using System.Globalization;
using System.Net;
using System.Text.Json;
using CommomTesteUtilities.IdEncryption;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Exceptions;
using Shouldly;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.Delete;

public class DeleteRecipeByIdTest : MyRecipeBookClassFixture
{
    private readonly string METHOD = "recipe";
    private readonly Guid _userIdentifier;
    private readonly string _recipeId;
    public DeleteRecipeByIdTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userIdentifier = factory.GetUserIdentifier();
        _recipeId = factory.GetRecipeId();
    }

    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGenerationBuilder.Build().Generate(_userIdentifier);

        var response = await DoDelete($"{METHOD}/{_recipeId}", token: token);
        
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        response = await DoGet($"{METHOD}/{_recipeId}", token: token);
        
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Recipe_NotFound(string culture)
    {
        var token = JwtTokenGenerationBuilder.Build().Generate(_userIdentifier);

        var id = IdEncrypterBuilder.Build().Encode(1000);

        var response = await DoDelete($"{METHOD}/{id}", token: token, culture: culture);
        
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        
        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("RECIPE_NOT_FOUND", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(
            () => errors.Count().ShouldBe(1),
            () => errors.First().GetString().ShouldBe(expectedMessage)
        );

    }
}