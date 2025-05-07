using System.Net;
using CommomTesteUtilities.Requests;
using CommonTestUtilities.Tokens;
using Shouldly;

namespace WebApi.Test.Recipe.Generate;

public class GenerateRecipeInvalidTokenTest : MyRecipeBookClassFixture
{
    private readonly string METHOD = "recipe/generate";
    public GenerateRecipeInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var request = RequestGenerateRecipeJsonBuilder.Build();

        var response = await DoPost(METHOD,request: request, token: "ïnvalid_token");
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Wihthout_Token()
    {
        var request = RequestGenerateRecipeJsonBuilder.Build();

        var response = await DoPost(METHOD,request: request, token: string.Empty);
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var token = JwtTokenGenerationBuilder.Build().Generate(Guid.NewGuid());

        var request = RequestGenerateRecipeJsonBuilder.Build();

        var response = await DoPost(METHOD,request: request, token: token);
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}