using API.Hubs;
using AutoMapper.Execution;
using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using DataAccess.Repo;
using Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OpenAI.Chat;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly IMessageRepo _messageRepo;

        public MessageController( UserManager<User> userManager, IHubContext<ChatHub> chatHub,IMessageRepo messageRepo)
        {
            
            _userManager = userManager;
            _chatHub = chatHub;
            _messageRepo = messageRepo;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            try
            {
                return Ok(await _messageRepo.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getMessageByUserId")]
        public async Task<IActionResult> GetAllByUserId(string receiverId)
        {

            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(senderId))
                return Unauthorized();

            var messages = await _messageRepo.GetByMessageByUsserId(senderId, receiverId);
            return Ok(messages);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppliById(int id)
        {

            try
            {
                return Ok(await _messageRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMessage(int id, [FromBody] MessageDto messageDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var message = await _messageRepo.GetById(id);
            if (message == null)
                return NotFound("Tin nhắn không tồn tại");

            // Chỉ cho phép người gửi chỉnh sửa tin nhắn
            if (message.SenderId != userId)
                return Forbid("Bạn không có quyền chỉnh sửa tin nhắn này");

            message.Content = messageDto.Message;
            message.Timestamp = DateTime.UtcNow; // Thêm thời gian cập nhật

            await _messageRepo.Update(message);

            // Gửi cập nhật realtime đến người nhận
            await _chatHub.Clients.User(message.ReceiverId)
                .SendAsync("UpdateMessage", id, message.Content);

            return Ok(message);
        }









        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppli(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var message = await _messageRepo.GetById(id);
            if (message == null)
                return NotFound();

            if (message.SenderId != userId)
                return Forbid("Bạn không có quyền xoá tin nhắn này");

            await _messageRepo.Delete(id);

            // 📌 Gửi thông báo xoá tin nhắn realtime
            await _chatHub.Clients.User(message.ReceiverId)
                .SendAsync("DeleteMessage", id);

            return Ok();


        }


        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageDto messageDto)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            
            if (senderId == null)
                return Unauthorized();
            await _chatHub.Clients.User(messageDto.ReceiverId)
        .SendAsync("ReceiveMessage", senderId, messageDto.Message);

            var message = new Business.Model.Message
            {
                SenderId = senderId,
                ReceiverId = messageDto.ReceiverId,
                Content = messageDto.Message,
                Timestamp = DateTime.UtcNow
            };
            await _messageRepo.Add(message);

            

            return Ok(message);
        }
    }
}
