using DMAdvantage.Client.Services;
using DMAdvantage.Client.Services.Implementations;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace DMAdvantage.Client.Helpers
{
    public static class ExtensionMethods
    {
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
            try
            {
                return await response.Content.ReadFromJsonAsync<T>(options);
            }
            catch
            {
                return default;
            }
        }

        public static async Task AddJwtHeader(this HttpRequestMessage request, ILocalStorageService localStorage)
        {
            var user = await localStorage.GetItem<LoginResponse>(AccountService.UserKey);
            var isApiUrl = request.RequestUri?.IsAbsoluteUri == false;
            if (user != null && isApiUrl)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
        }

        public static string PrintInt(this int i)
        {
            if (i >= 0)
                return $"+{i}";
            else
                return i.ToString();
        }

        private static async Task HandleErrors(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }
    }
}
