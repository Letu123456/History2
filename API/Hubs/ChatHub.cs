using AutoMapper.Execution;
using Business;
using Business.Model;

using Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using OpenAI.Chat;
using System.Text.RegularExpressions;

namespace API.Hubs
{
    public class ChatHub:Hub
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public ChatHub(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Gửi tin nhắn riêng tư
        public async Task SendMessage(string receiverId, string message)
        {
            var senderId = Context.UserIdentifier; // Lấy ID của người gửi từ Identity

            if (string.IsNullOrEmpty(senderId)) return;

            
            
            

            // Gửi tin nhắn đến người nhận (nếu đang online)
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message);
        }

        // Khi người dùng kết nối, lưu ConnectionId
        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            if (user != null)
            {
                var userId = _userManager.GetUserId(user);
                if (!string.IsNullOrEmpty(userId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                }
            }
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = Context.User;
            if (user != null)
            {
                var userId = _userManager.GetUserId(user);
                if (!string.IsNullOrEmpty(userId))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
