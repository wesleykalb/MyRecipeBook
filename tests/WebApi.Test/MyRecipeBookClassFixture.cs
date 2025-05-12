using System.Net.Http.Json;
using System.Net.Http.Headers;
using Castle.Components.DictionaryAdapter.Xml;

namespace WebApi.Test
{
    public class MyRecipeBookClassFixture : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        public MyRecipeBookClassFixture(CustomWebApplicationFactory factory) => _httpClient = factory.CreateClient();

        protected async Task<HttpResponseMessage> DoPost(string method, object request, string culture = "en", string token = "")
        {
            ChangeRequestCulture(culture);
            AuthorizeRequest(token);
            return await _httpClient.PostAsJsonAsync(method, request);
        }

        protected async Task<HttpResponseMessage> DoGet(string method, string token = "", string culture = "en")
        {
            ChangeRequestCulture(culture);
            AuthorizeRequest(token);

            return await _httpClient.GetAsync(method);
        }
        protected async Task<HttpResponseMessage> DoDelete(string method, string token, string culture = "en")
        {
            ChangeRequestCulture(culture);
            AuthorizeRequest(token);

            return await _httpClient.DeleteAsync(method);
        }
        protected async Task<HttpResponseMessage> DoPut(string method, object request, string token, string culture = "en")
        {
            ChangeRequestCulture(culture);
            AuthorizeRequest(token);

            return await _httpClient.PutAsJsonAsync(method, request);
        }

        protected async Task<HttpResponseMessage> DoPostFormData(string method, object request, string token, string culture = "en")
        {
            ChangeRequestCulture(culture);
            AuthorizeRequest(token);

            var multPart = new MultipartFormDataContent();

            var properties = request.GetType().GetProperties().ToList();
    
            foreach (var property in properties)
            {
                var valueProperty = property.GetValue(request);

                if (string.IsNullOrWhiteSpace(valueProperty?.ToString()))
                    continue;

                if (valueProperty is System.Collections.IList list)
                    AddListToMultiPartContent(multPart, property.Name, list);
                else
                    multPart.Add(new StringContent(valueProperty.ToString()!), property.Name);
            }

            return await _httpClient.PostAsync(method, multPart);
        }

        private void ChangeRequestCulture(string culture)
        {
            if (_httpClient.DefaultRequestHeaders.Contains("Accept-Language"))
                _httpClient.DefaultRequestHeaders.Remove("Accept-Language");

            _httpClient.DefaultRequestHeaders.Add("Accept-Language", culture);
        }

        private void AuthorizeRequest(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return;
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private void AddListToMultiPartContent(
        MultipartFormDataContent multipartContent,
        string propertyName,
        System.Collections.IList list)
        {
            var itemType = list.GetType().GetGenericArguments().Single();

            if (itemType.IsClass && itemType != typeof(string))
            {
                AddClassListToMultiPartContent(multipartContent, propertyName, list);
            }
            else
            {
                foreach (var property in list)
                {
                    multipartContent.Add(new StringContent(property.ToString()!), propertyName);
                }
            }
        }

        private void AddClassListToMultiPartContent(
        MultipartFormDataContent multipartContent,
        string propertyName,
        System.Collections.IList list
        )
        {
            var index = 0;

            foreach (var property in list)
            {
                var classPropertyInfo = property.GetType().GetProperties().ToList();

                foreach(var prop in classPropertyInfo)
                {
                    var value = prop.GetValue(property, null);
                    multipartContent.Add(new StringContent(value!.ToString()!), $"{propertyName}[{index}][{prop.Name}]");
                }
                index++;
            }
        }
    }
}
