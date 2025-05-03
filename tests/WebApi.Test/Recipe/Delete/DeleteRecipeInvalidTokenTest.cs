using System.Net;
using CommomTesteUtilities.IdEncryption;
using CommonTestUtilities.Tokens;
using Shouldly;

namespace WebApi.Test.Recipe.Delete;

public class DeleteRecipeInvalidTokenTest : MyRecipeBookClassFixture
{
    private readonly string METHOD = "recipe";
    public DeleteRecipeInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var id = IdEncrypterBuilder.Build().Encode(1);

        var response = await DoDelete($"{METHOD}/{id}", token: "ïnvalid_token");
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Wihthout_Token()
    {
        var id = IdEncrypterBuilder.Build().Encode(1);

        var response = await DoDelete($"{METHOD}/{id}", token: string.Empty);
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var token = JwtTokenGenerationBuilder.Build().Generate(Guid.NewGuid());

        var id = IdEncrypterBuilder.Build().Encode(1);

        var response = await DoDelete($"{METHOD}/{id}", token: token);
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}