using System.Net;
using System.Text.Json;
using CommonTestUtilities.Tokens;
using Shouldly;
using WebApi.Test;

namespace UsesCases.Test.User.Profile;

public class GetUserProfileTest : MyRecipeBookClassFixture
{
    private readonly string METHOD = "user";
    private readonly string _name;
    private readonly string _email;
    private readonly Guid _guid;
    public GetUserProfileTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _name = factory.GetName();
        _email = factory.GetEmail();
        _guid = factory.GetUserIdentifier();
    }

    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGenerationBuilder.Build().Generate(_guid);

        var response = await DoGet(METHOD, token: token);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var name = responseData.RootElement.GetProperty("name").GetString();
        var email = responseData.RootElement.GetProperty("email").GetString();

        name.ShouldNotBeNullOrWhiteSpace();
        name.ShouldBe(_name);
        
        email.ShouldNotBeNullOrWhiteSpace();
        email.ShouldBe(_email);
    }
    
}