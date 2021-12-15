using System.Net.Http.Headers;

namespace DMAdvantage.Client.Services
{
    public interface IHttpService
    {
        Task<T?> Get<T>(string uri);
        Task<(T?, HttpResponseHeaders)> GetWithResponseHeader<T>(string uri);
        Task<T?> GetWithHeader<T>(string uri, string key, string value);
        Task Post(string uri, object value);
        Task<T?> Post<T>(string uri, object value);
        Task Put(string uri, object value);
        Task<T?> Put<T>(string uri, object value);
        Task Delete(string uri);
        Task<T?> Delete<T>(string uri);
    }
}
