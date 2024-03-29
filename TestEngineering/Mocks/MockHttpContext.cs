﻿using DMAdvantage.Shared.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using System.Security.Claims;

namespace TestEngineering.Mocks
{
    public class MockHttpContext : HttpContext
    {
        public static readonly string CurrentUser = "testuser@email.com";

        public MockHttpContext()
        {
            var mockFeatureCollection = new Mock<IFeatureCollection>();
            Features = mockFeatureCollection.Object;
            Request = new MockHttpRequest(this);
            Response = new MockHttpResponse(this);
            var mockConnectionInfo = new Mock<ConnectionInfo>();
            Connection = mockConnectionInfo.Object;
            var mockWebSocketManager = new Mock<WebSocketManager>();
            WebSockets = mockWebSocketManager.Object;
            var mockClaimsPrincipal = new Mock<ClaimsPrincipal>();
            mockClaimsPrincipal.Setup(x => x.Identity!.Name).Returns(CurrentUser);
            User = mockClaimsPrincipal.Object;
        }

        public override IFeatureCollection Features { get; }

        public override HttpRequest Request { get; }

        public override HttpResponse Response { get; }

        public override ConnectionInfo Connection { get; }

        public override WebSocketManager WebSockets { get; }

        public sealed override ClaimsPrincipal User { get; set; }
        public override IDictionary<object, object?> Items { get; set; } = new Dictionary<object, object?>();
        public override IServiceProvider RequestServices { get; set; } = new ServiceProvidersFeature().RequestServices;
        public override CancellationToken RequestAborted { get; set; }
        public override string TraceIdentifier { get; set; } = string.Empty;
        public override ISession Session { get; set; } = new DefaultSessionFeature().Session;

        public override void Abort()
        {
            
        }
    }
}
