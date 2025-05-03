using System.Globalization;
using System.Net;
using System.Text.Json;
using Azure.Core;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Exceptions;
using MyRecipeBook.tests.CommomTesteUtilities.Requests;
using Shouldly;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.Update;

public class UpdateUserTest : MyRecipeBookClassFixture
{
    private readonly string METHOD = "user";
    private readonly Guid _userIdentifier;
    public UpdateUserTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userIdentifier = factory.GetUserIdentifier();
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestUpdateUserJsonBuilder.Build();

        var token = JwtTokenGenerationBuilder.Build().Generate(_userIdentifier);

        var response = await DoPut(METHOD, request, token);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Empty_Name(string culture)
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = string.Empty;

        var token = JwtTokenGenerationBuilder.Build().Generate(_userIdentifier);

        var response = await DoPut(METHOD, request, token, culture);

        await using var reponseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(reponseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(
            () => errors.Count().ShouldBe(1),
            () => errors.First().GetString().ShouldBe(expectedMessage)
        );
    }
}
