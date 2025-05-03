using System.Globalization;
using System.Net;
using System.Text.Json;
using CommomTesteUtilities.Requests;
using CommonTestUtilities.Tokens;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.ChangePassword;

public class ChangePasswordTest : MyRecipeBookClassFixture
{
    private readonly string METHOD = "user/change-password";
    private readonly string _email;
    private readonly string _password;
    private readonly Guid _userIdentifier;
    public ChangePasswordTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _email = factory.GetEmail();
        _password = factory.GetPassword();
        _userIdentifier = factory.GetUserIdentifier();
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestChangePasswordJsonBuilder.Build();
        request.Password = _password;

        var token = JwtTokenGenerationBuilder.Build().Generate(_userIdentifier);

        var result = await DoPut(METHOD, request, token);
        
        result.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var loginRequest = new RequestLoginJson
        {
            Email = _email,
            Password = _password
        };
        
        result = await DoPost("login", loginRequest);
        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        loginRequest.Password = request.NewPassword;

        result = await DoPost("login", loginRequest);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_NewPassword_Empty(string culture)
    {
        var request =  new RequestChangePasswordJson
        {
            Password = _password,
            NewPassword = string.Empty
        };
        var token = JwtTokenGenerationBuilder.Build().Generate(_userIdentifier);

        var result = await DoPut(METHOD, request, token, culture);
        
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var response = await result.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(response);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("PASSWORD_EMPTY", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(
            () => errors.Count().ShouldBe(1),
            () => errors.First().GetString().ShouldBe(expectedMessage)
        );
    }
}