using System.Net;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Tests.CommomTesteUtilities.Requests;
using Shouldly;

namespace WebApi.Test.Recipe.Filter;

public class FilterRecipeInvalidTokenTest : MyRecipeBookClassFixture
{
    private readonly string METHOD = "recipe/filter";
    public FilterRecipeInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var request = RequestFilterRecipeJsonBuilder.Build();

        var response = await DoPost(METHOD, request, token: "ïnvalid_token");
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Wihthout_Token()
    {
        var request = RequestFilterRecipeJsonBuilder.Build();

        var response = await DoPost(METHOD, request, token: string.Empty);
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var request = RequestFilterRecipeJsonBuilder.Build();

        var token = JwtTokenGenerationBuilder.Build().Generate(Guid.NewGuid());

        var response = await DoPost(METHOD, request, token: token);
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}