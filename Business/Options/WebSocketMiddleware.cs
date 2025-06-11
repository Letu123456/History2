using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Business.Options
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketManager _wsManager;

        public WebSocketMiddleware(RequestDelegate next, WebSocketManager wsManager)
        {
            _next = next;
            _wsManager = wsManager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var user = context.User;
                    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (string.IsNullOrEmpty(userId))
                    {
                        context.Response.StatusCode = 401; // Unauthorized
                        return;
                    }

                    using var socket = await context.WebSockets.AcceptWebSocketAsync();
                    _wsManager.AddSocket(userId, socket);

                    var buffer = new byte[1024 * 4];
                    while (socket.State == WebSocketState.Open)
                    {
                        var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (result.CloseStatus.HasValue)
                        {
                            _wsManager.RemoveSocket(userId);
                            await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
