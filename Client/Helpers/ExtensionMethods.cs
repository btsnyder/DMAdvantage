using DMAdvantage.Client.Services;
using DMAdvantage.Client.Services.Implementations;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

namespace DMAdvantage.Client.Helpers
{
    public static class ExtensionMethods
    {
        public static NameValueCollection QueryString(this NavigationManager navigationManager)
        {
            return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
        }

        public static string? QueryString(this NavigationManager navigationManager, string key)
        {
            return navigationManager.QueryString()[key];
        }

        public static object? GetPropertyByName(this object obj, string name)
        {
            var info = obj.GetType().GetProperty(name);
            return info?.GetValue(obj);
        }

        public static void SetPropertyByName(this object obj, string name, object value)
        {
            var info = obj.GetType().GetProperty(name);
            info?.SetValue(obj, value);
        }

        public static async Task<bool> ProcessResponseValidity(this HttpResponseMessage response, NavigationManager navigationManager)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                navigationManager.NavigateTo("account/logout");
                return false;
            }

            await HandleErrors(response);

            return true;
        }

        public static async Task<T?> ParseContent<T>(this HttpResponseMessage response)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new StringConverter());
            return await response.Content.ReadFromJsonAsync<T>(options);
        }

        public static async Task AddJwtHeader(this HttpRequestMessage request, ILocalStorageService localStorage)
        {
            var user = await localStorage.GetItem<LoginResponse>(AccountService.UserKey);
            var isApiUrl = request.RequestUri?.IsAbsoluteUri == false;
            if (user != null && isApiUrl)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
        }

        public static async Task HandleErrors(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                throw new Exception(error == null ? "Unknown error occured." : error["message"]);
            }
        }
    }
}
