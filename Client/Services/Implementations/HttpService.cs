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
            return await ProcessRequest<T>(request);
        }

        public async Task<(T?, HttpResponseHeaders)> GetWithResponseHeader<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await ProcessRequestWithHeaders<T>(request);
        }

        public async Task<T?> GetWithHeader<T>(string uri, string key, string value)
        {
            var request = CreateRequest(HttpMethod.Post, uri, new Dictionary<string, string> { { key, value } });
            return await ProcessRequest<T>(request);
        }

        public async Task Post(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Post, uri, value);
            await ProcessRequest(request);
        }

        public async Task<T?> Post<T>(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Post, uri, value);
            return await ProcessRequest<T>(request);
        }

        public async Task Put(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Put, uri, value);
            await ProcessRequest(request);
        }

        public async Task<T?> Put<T>(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Put, uri, value);
            return await ProcessRequest<T>(request);
        }

        public async Task Delete(string uri)
        {
            var request = CreateRequest(HttpMethod.Delete, uri);
            await ProcessRequest(request);
        }

        public async Task<T?> Delete<T>(string uri)
        {
            var request = CreateRequest(HttpMethod.Delete, uri);
            return await ProcessRequest<T>(request);
        }

        private static HttpRequestMessage CreateRequest(HttpMethod method, string uri, object? value = null)
        {
            var request = new HttpRequestMessage(method, uri);
            if (value != null)
                request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return request;
        }

        private async Task<HttpResponseMessage> SendAuthorizedRequest(HttpRequestMessage request)
        {
            await request.AddJwtHeader(_localStorageService);

            return await _httpClient.SendAsync(request);
        }

        private async Task ProcessRequest(HttpRequestMessage request)
        {
            using var response = await SendAuthorizedRequest(request);

            await response.ProcessResponseValidity(_navigationManager);
        }

        private async Task<T?> ProcessRequest<T>(HttpRequestMessage request)
        {
            using var response = await SendAuthorizedRequest(request);

            var valid = await response.ProcessResponseValidity(_navigationManager);
            
            if (!valid)
                return default;

            return await response.ParseContent<T>();
        }

        private async Task<(T?, HttpResponseHeaders)> ProcessRequestWithHeaders<T>(HttpRequestMessage request)
        {
            using var response = await SendAuthorizedRequest(request);

            var valid = await response.ProcessResponseValidity(_navigationManager);

            if (!valid)
                return default;

            var content = await response.ParseContent<T>();
            return (content, response.Headers);
        }
    }
}
