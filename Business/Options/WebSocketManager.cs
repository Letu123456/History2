using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Business.Model;

namespace Business.Options
{
    public class WebSocketManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _userSockets = new();
        
         

        public void AddSocket(string userId, WebSocket socket)
        {
            _userSockets[userId] = socket; // Lưu socket theo UserId
        }

        public async Task SendMessageToUserAsync(string userId, string message)
        {
            // 🔹 Lưu thông báo vào database trước
            

            // 🔹 Gửi thông báo qua WebSocket nếu user đang online
            if (_userSockets.TryGetValue(userId, out WebSocket socket))
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public void RemoveSocket(string userId)
        {
            _userSockets.TryRemove(userId, out _);
        }
    }
}
