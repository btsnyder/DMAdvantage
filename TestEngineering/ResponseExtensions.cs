using System.Text.Json;

namespace TestEngineering
{
    public static class ResponseExtensions
    {
        public static async Task<T> ParseEntity<T>(this HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var entity = JsonSerializer.Deserialize<T>(json, options);
            if (entity == null)
                throw new Exception("Could not parse reponse");
            return entity;
        }

        public static async Task<List<T>> ParseEntityList<T>(this HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var entities = JsonSerializer.Deserialize<List<T>>(json, options);
            if (entities == null)
                throw new Exception("Could not parse reponse");
            return entities;
        }
    }
}
