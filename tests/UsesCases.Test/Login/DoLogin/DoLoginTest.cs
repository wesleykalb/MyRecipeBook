using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CommomTesteUtilities.Requests;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using WebApi.Test;
using WebApi.Test.InlineData;

namespace UsesCases.Test.Login.DoLogin
{
    public class DoLoginTest : MyRecipeBookClassFixture
    {
        private readonly string method = "login";

        private readonly string _password;

        private readonly string _email;

        private readonly string _name;

        public DoLoginTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _email = factory.GetEmail();
            _password = factory.GetPassword();
            _name = factory.GetName();
        }

        [Fact]
        public async Task Success()
        {
            var request = new RequestLoginJson
            {
                Email = _email,
                Password = _password
            };

            var response = await DoPost(method, request);

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            
            await using var responseBody = await response.Content.ReadAsStreamAsync();

            var result = await JsonDocument.ParseAsync(responseBody);

            var resultName = result.RootElement.GetProperty("name").GetString();

            resultName.ShouldNotBeNullOrWhiteSpace();

            resultName.ShouldBe(_name);
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_Empty_Name(string culture)
        {
            var request = RequestLoginJsonBuilder.Build();

            var response = await DoPost(method, request, culture);

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

            await using var reponseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(reponseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            var expectedMessage = ResourceMessagesException.ResourceManager.GetString("EMAIL_OR_PASSWORD_INVALID", new CultureInfo(culture));

            errors.ShouldSatisfyAllConditions(
                () => errors.Count().ShouldBe(1),
                () => errors.First().GetString().ShouldBe(expectedMessage)
            );
        }
    }
}
