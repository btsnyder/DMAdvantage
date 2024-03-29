﻿using Microsoft.AspNetCore.Http;
using Moq;

namespace TestEngineering.Mocks
{
    public class MockHttpRequest : HttpRequest
    {
        public MockHttpRequest(HttpContext context)
        {
            HttpContext = context;
            var mockRequestCookieCollection = new Mock<IRequestCookieCollection>();
            Cookies = mockRequestCookieCollection.Object;
        }

        public override HttpContext HttpContext { get; }

        public override string Method { get; set; } = string.Empty;
        public override string Scheme { get; set; } = string.Empty;
        public override bool IsHttps { get; set; }
        public override HostString Host { get; set; }
        public override PathString PathBase { get; set; }
        public override PathString Path { get; set; }
        public override QueryString QueryString { get; set; }
        public override IQueryCollection Query { get; set; } = new QueryCollection();
        public override string Protocol { get; set; } = string.Empty;

        public override IHeaderDictionary Headers => new HeaderDictionary();

        public sealed override IRequestCookieCollection Cookies { get; set; }
        public override long? ContentLength { get; set; }
        public override string? ContentType { get; set; }
        public override Stream Body { get; set; } = new MemoryStream();

        public override bool HasFormContentType => true;

        public override IFormCollection Form { get; set; } = FormCollection.Empty;

        public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Form);
        }
    }
}
