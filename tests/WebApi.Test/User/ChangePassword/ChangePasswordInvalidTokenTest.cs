using System.Net;
using CommonTestUtilities.Tokens;
using Microsoft.Identity.Client;
using MyRecipeBook.Communication.Requests;
using Shouldly;

namespace WebApi.Test.User.ChangePassword;

public class ChangePasswordInvalidTokenTest : MyRecipeBookClassFixture
{
    private readonly string METHOD = "user/change-password";
    public ChangePasswordInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }
    [Fact]
    public async Task Error_Invalid_Token()
    {
        var request = new RequestChangePasswordJson();

        var result = await DoPut(METHOD, request, token: "invalid-token");
        
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var request = new RequestChangePasswordJson();

        var result = await DoPut(METHOD, request, token: string.Empty);
        
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var token = JwtTokenGenerationBuilder.Build().Generate(Guid.NewGuid());

        var request = new RequestChangePasswordJson();

        var result = await DoPut(METHOD, request, token);
        
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

}