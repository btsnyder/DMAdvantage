using System.Net.Http.Headers;

namespace DMAdvantage.Client.Services
{
    public interface IHttpService
    {
        Task<T?> Get<T>(string uri) where T : class;
        Task<(T?, HttpResponseHeaders?)> GetWithResponseHeader<T>(string uri, CancellationToken? token) where T : class;
        Task<T?> GetWithHeader<T>(string uri, string key, string value) where T : class;
        Task Post(string uri, object value);
        Task<T?> Post<T>(string uri, object value) where T : class;
        Task Put(string uri, object value);
        Task<T?> Put<T>(string uri, object value) where T : class;
        Task Delete(string uri);
        Task<T?> Delete<T>(string uri) where T : class;
    }
}
