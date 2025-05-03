using System.Net;
using Azure;
using Azure.Core;
using CommonTestUtilities.Tokens;
using MyRecipeBook.tests.CommomTesteUtilities.Requests;
using Shouldly;

namespace WebApi.Test.User.Update;

public class UpdateUserInvalidTokenTest : MyRecipeBookClassFixture
{
    private readonly string METHOD = "user";
    public UpdateUserInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Error_Invalid_Token()
    {
        var request = RequestUpdateUserJsonBuilder.Build();

        var response = await DoPut(METHOD, request, "invalid_token");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var request = RequestUpdateUserJsonBuilder.Build();

        var response = await DoPut(METHOD, request, string.Empty);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Token_With_User_Not_Found()
    {
        var request = RequestUpdateUserJsonBuilder.Build();

        var token = JwtTokenGenerationBuilder.Build().Generate(Guid.NewGuid());

        var response = await DoPut(METHOD, request, token);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}