using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CommomTesteUtilities.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.Register
{
    public class RegisterUserTest : MyRecipeBookClassFixture
    {
        private readonly string method = "user";

        public RegisterUserTest(CustomWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task Success()
        { 
            var request = RequestRegisterUserJsonBuilder.Build();

            var response = await DoPost(method, request);

            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            await using var reponseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(reponseBody);

            var responseDataName = responseData.RootElement.GetProperty("name").GetString();

            var resultToken = responseData.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString();

            responseData.RootElement.GetProperty("name").GetString().ShouldSatisfyAllConditions(
                () => responseDataName.ShouldNotBeNull(),
                () => responseDataName.ShouldBe(request.Name)
            );
            resultToken.ShouldNotBeNullOrWhiteSpace();
        }
        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_Empty_Name(string culture)
        {
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Name = string.Empty;

            var response = await DoPost(method, request, culture);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            await using var reponseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(reponseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            var expectedMessage = ResourceMessagesException.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));

            errors.ShouldSatisfyAllConditions(
                () => errors.Count().ShouldBe(1),
                () => errors.First().GetString().ShouldBe(expectedMessage)
            );
        }

    }
}
