using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Extensions;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using DMAdvantage.Shared.Services.Kafka;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using TestEngineering;
using TestEngineering.Data;
using TestEngineering.Mocks;
using Xunit;

namespace DMAdvantage.IntegrationTests.Controllers
{
    public class WebSocketTests
    {
        private readonly TestServer _server;

        public WebSocketTests()
        {
            var factory = new TestServerFactory();
            _server = factory.Create();
        }

        [Fact]
        public async Task Get_AuthenticatedPageForUnauthenticatedUser_Unauthorized()
        {
            var wsClient = _server.CreateWebSocketClient();
            var wsUri = new UriBuilder(_server.BaseAddress) { Scheme = "ws" }.Uri;

            Func<Task> act = async () => await wsClient.ConnectAsync(wsUri, CancellationToken.None);
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Theory]
        [ClassData(typeof(KafkaTopicsTestData))]
        public async Task GetWebSocket_WithProducedMessage_Success(string topic)
        {
            var wsClient = _server.CreateWebSocketClient();
            var wsUri = new UriBuilder($"{_server.BaseAddress}api/ws/{topic}") { Scheme = "ws" }.Uri;
            
            var webSocket = await wsClient.ConnectAsync(wsUri, CancellationToken.None);
            webSocket.State.Should().Be(WebSocketState.Open);

            var messages = new List<KafkaMessage>();
            AddSocketMessage(webSocket, messages);
            var producer = new KafkaProducer();
            producer.Start();
            producer.SendMessage(new KafkaMessage { Topic = topic, User = "testuser", Value = KafkaValues.Updated });
            await Task.Delay(1000);
            producer.Stop();
            messages.Should().ContainSingle(m => m.Value == KafkaValues.Updated);
        }

        private static async void AddSocketMessage(WebSocket webSocket, List<KafkaMessage> messages)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
            receiveResult.CloseStatus.Should().BeNull();
            var serializedMessage = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
            messages.Add(KafkaMessage.Deserialize(serializedMessage));
            await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Closing from test.", CancellationToken.None);
        }
    }
}
