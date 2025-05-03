using System.Buffers;
using System.Globalization;
using System.Net;
using System.Text.Json;
using CommomTesteUtilities.IdEncryption;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Exceptions;
using Shouldly;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.Dashboard;

public class GetDashboardTest : MyRecipeBookClassFixture
{
    private readonly string METHOD = "dashboard";
    private readonly Guid _userIdentifier;
    public GetDashboardTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userIdentifier = factory.GetUserIdentifier();
    }

    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGenerationBuilder.Build().Generate(_userIdentifier);

        var response = await DoGet(METHOD, token: token);
        
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("recipes").EnumerateArray().Count().ShouldBeGreaterThan(0);
    }

}