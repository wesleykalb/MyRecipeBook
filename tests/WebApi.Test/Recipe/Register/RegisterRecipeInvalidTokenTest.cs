using System.Net;
using CommomTesteUtilities.Requests;
using CommonTestUtilities.Tokens;
using Shouldly;

namespace WebApi.Test.Recipe.Register;

public class RegisterRecipeInvalidTokenTest : MyRecipeBookClassFixture
{
    private readonly string METHOD = "recipe";
    public RegisterRecipeInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var request = RequestRecipeJsonBuilder.Build();

        var response = await DoPost(METHOD, request, token: "ïnvalid_token");
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Wihthout_Token()
    {
        var request = RequestRecipeJsonBuilder.Build();

        var response = await DoPost(METHOD, request, token: string.Empty);
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var request = RequestRecipeJsonBuilder.Build();

        var token = JwtTokenGenerationBuilder.Build().Generate(Guid.NewGuid());

        var response = await DoPost(METHOD, request, token: token);
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}