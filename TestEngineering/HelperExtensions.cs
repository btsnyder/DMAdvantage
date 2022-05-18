using Microsoft.AspNetCore.TestHost;
using System.Net.Http.Headers;

namespace TestEngineering
{
    public static class HelperExtensions
    {
        public static async Task<HttpClient> CreateAuthenticatedClientAsync(this TestServer server)
        {
            var client = server.CreateClient();
            var token = await client.CreateToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
}
