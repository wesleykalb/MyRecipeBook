using System.Net;
using CommomTesteUtilities.IdEncryption;
using CommonTestUtilities.Tokens;
using Shouldly;

namespace WebApi.Test.Recipe.Dashboard;

public class GetDashboardIdInvalidTokenTest : MyRecipeBookClassFixture
{
    private readonly string METHOD = "dashboard";
    public GetDashboardIdInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var id = IdEncrypterBuilder.Build().Encode(1);

        var response = await DoGet(METHOD, token: "ïnvalid_token");
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Wihthout_Token()
    {
        var id = IdEncrypterBuilder.Build().Encode(1);

        var response = await DoGet(METHOD, token: string.Empty);
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var token = JwtTokenGenerationBuilder.Build().Generate(Guid.NewGuid());

        var id = IdEncrypterBuilder.Build().Encode(1);

        var response = await DoGet(METHOD, token: token);
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}