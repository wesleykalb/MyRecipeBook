using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CommomTesteUtilities.Requests;
using Microsoft.AspNetCore.Mvc.Testing;
using MyRecipeBook.Exceptions;
using Shouldly;
using WebApi.Test.InlineData;
using Xunit;

namespace WebApi.Test.User.Register
{
    public class RegisterUserTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;

        public RegisterUserTest(CustomWebApplicationFactory factory) => _httpClient = factory.CreateClient();
        

        [Fact]
        public async Task Success()
        { 
            var request = RequestRegisterUserJsonBuilder.Build();

            var response = await _httpClient.PostAsJsonAsync("User", request);

            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            await using var reponseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(reponseBody);

            var responseDataName = responseData.RootElement.GetProperty("name").GetString();

            responseData.RootElement.GetProperty("name").GetString().ShouldSatisfyAllConditions(
                () => responseDataName.ShouldNotBeNull(),
                () => responseDataName.ShouldBe(request.Name)
            );
        }
        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_Empty_Name(string culture)
        {
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Name = string.Empty;

            if (_httpClient.DefaultRequestHeaders.Contains("Accept-Language"))
                _httpClient.DefaultRequestHeaders.Remove("Accept-Language");

            _httpClient.DefaultRequestHeaders.Add("Accept-Language", culture);

            var response = await _httpClient.PostAsJsonAsync("User", request);

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
