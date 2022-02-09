using Microsoft.AspNetCore.Http;
using Moq;

namespace TestEngineering.Mocks
{
    public class MockHttpResponse : HttpResponse
    {
        public MockHttpResponse(HttpContext context)
        {
            HttpContext = context;
            var mockResponseCookies = new Mock<IResponseCookies>();
            Cookies = mockResponseCookies.Object;
        }

        public override HttpContext HttpContext { get; }

        public override int StatusCode { get; set; }

        public override IHeaderDictionary Headers => new HeaderDictionary();

        public override Stream Body { get; set; } = new MemoryStream();
        public override long? ContentLength { get; set; }
        public override string ContentType { get; set; } = string.Empty;

        public override IResponseCookies Cookies { get; }

        public override bool HasStarted => false;

        public override void OnCompleted(Func<object, Task> callback, object state)
        {
            callback.Invoke(state);
        }

        public override void OnStarting(Func<object, Task> callback, object state)
        {
            callback.Invoke(state);
        }

        public override void Redirect(string location, bool permanent)
        {
            throw new NotImplementedException();
        }
    }
}
