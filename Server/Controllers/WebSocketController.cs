using DMAdvantage.Shared.Services.Kafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace DMAdvantage.Server.Controllers
{
    [Route("api/ws")]
    public class WebSocketController : Controller
    {
        private readonly KafkaConsumer _consumer;

        public WebSocketController(KafkaConsumer consumer)
        {
            _consumer = consumer;
        }

        ~WebSocketController()
        {
            _consumer?.Stop();
        }

        [HttpGet]
        [Route("{topic}")]
        public async Task Get(string topic)
        {
            _consumer.Start(topic);
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await Listen(webSocket, topic);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task Listen(WebSocket webSocket, string topic)
        {
            var id = _consumer!.LastConsumed.Item1;
            Task.Run(() => _consumer.Listen(topic));
            Task.Run(() => ListenToSocket(webSocket));
            while (webSocket.State == WebSocketState.Open)
            {
                await Task.Delay(100);
                if (_consumer.LastConsumed.Item1 != id)
                {
                    id = _consumer.LastConsumed.Item1;
                    var value = _consumer.LastConsumed.Item2.Serialize();
                    await webSocket.SendAsync(
                        new ArraySegment<byte>(value, 0, value.Length),
                        WebSocketMessageType.Binary,
                        true,
                        CancellationToken.None);
                }
            }
            _consumer.Stop();
        }

        private static async Task ListenToSocket(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

        }
    }
}
