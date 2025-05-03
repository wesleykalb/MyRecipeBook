using System.Net;
using CommonTestUtilities.Tokens;
using Shouldly;
using WebApi.Test;

namespace MyRecipeBook.Test.WebApi.User.Profile;

public class GetUserProfileInvalidTokenTest : MyRecipeBookClassFixture
{
    private readonly string METHOD = "user";
    public GetUserProfileInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var response = await DoGet(METHOD, "TokenInvalid");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var response = await DoGet(METHOD, string.Empty);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Error_Token_With_User_Not_Found()
    {
        var token = JwtTokenGenerationBuilder.Build().Generate(Guid.NewGuid());

        var response = await DoGet(METHOD, token);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}