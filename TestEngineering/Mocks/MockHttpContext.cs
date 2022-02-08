using DMAdvantage.Shared.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using System.Security.Claims;

namespace TestEngineering.Mocks
{
    public class MockHttpContext : HttpContext
    {
        public readonly static User CurrentUser = new() { UserName = "testuser@email.com", Email = "testuser@email.com" };

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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            mockClaimsPrincipal.Setup(x => x.Identity.Name).Returns(CurrentUser.UserName);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            User = mockClaimsPrincipal.Object;
        }

        public override IFeatureCollection Features { get; }

        public override HttpRequest Request { get; }

        public override HttpResponse Response { get; }

        public override ConnectionInfo Connection { get; }

        public override WebSocketManager WebSockets { get; }

        public override ClaimsPrincipal User { get; set; }
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
