using DMAdvantage.Client.Helpers;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace DMAdvantage.Client.Services.Implementations
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly ILocalStorageService _localStorageService;

        public HttpService(
            HttpClient httpClient,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
        }

        public async Task<T?> Get<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await SendRequest<T>(request);
        }

        public async Task<T?> GetWithHeader<T>(string uri, string key, string value)
        {
            var request = CreateRequest(HttpMethod.Post, uri, new Dictionary<string, string> { { key, value } });
            return await SendRequest<T>(request);
        }

        public async Task Post(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Post, uri, value);
            await SendRequest(request);
        }

        public async Task<T?> Post<T>(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Post, uri, value);
            return await SendRequest<T>(request);
        }

        public async Task Put(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Put, uri, value);
            await SendRequest(request);
        }

        public async Task<T?> Put<T>(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Put, uri, value);
            return await SendRequest<T>(request);
        }

        public async Task Delete(string uri)
        {
            var request = CreateRequest(HttpMethod.Delete, uri);
            await SendRequest(request);
        }

        public async Task<T?> Delete<T>(string uri)
        {
            var request = CreateRequest(HttpMethod.Delete, uri);
            return await SendRequest<T>(request);
        }

        private static HttpRequestMessage CreateRequest(HttpMethod method, string uri, object? value = null)
        {
            var request = new HttpRequestMessage(method, uri);
            if (value != null)
                request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return request;
        }

        private async Task SendRequest(HttpRequestMessage request)
        {
            await AddJwtHeader(request);

            using var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _navigationManager.NavigateTo("account/logout");
                return;
            }

            await HandleErrors(response);
        }

        private async Task<T?> SendRequest<T>(HttpRequestMessage request)
        {
            await AddJwtHeader(request);

            using var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _navigationManager.NavigateTo("account/logout");
                return default;
            }

            await HandleErrors(response);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new StringConverter());
            return await response.Content.ReadFromJsonAsync<T>(options);
        }

        private async Task AddJwtHeader(HttpRequestMessage request)
        {
            var user = await _localStorageService.GetItem<LoginResponse>(AccountService.UserKey);
            var isApiUrl = request.RequestUri?.IsAbsoluteUri == false;
            if (user != null && isApiUrl)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
        }

        private static async Task HandleErrors(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                throw new Exception(error == null ? "Unknown error occured." : error["message"]);
            }
        }
    }
}
