using API.Controllers;
using API.Hubs;
using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using System;

namespace API.Tests
{
    public class MessageControllerTests
    {
        private readonly Mock<IMessageRepo> _mockMessageRepo = new();
        private readonly Mock<IHubContext<ChatHub>> _mockHubContext = new();
        private readonly Mock<IUserStore<User>> _mockUserStore = new();
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<IClientProxy> _mockClientProxy = new();
        private readonly MessageController _controller;

        public MessageControllerTests()
        {
            _mockUserManager = new Mock<UserManager<User>>(
                _mockUserStore.Object, null, null, null, null, null, null, null, null);

            _controller = new MessageController(
                _mockUserManager.Object,
                _mockHubContext.Object,
                _mockMessageRepo.Object
            );
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            _mockMessageRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Message>());
            var result = await _controller.GetAll();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsOk()
        {
            _mockMessageRepo.Setup(r => r.GetById(1)).ReturnsAsync(new Message { Id = 1 });
            var result = await _controller.GetAppliById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<Message>(okResult.Value);
        }

        [Fact]
        public async Task SendMessage_ValidInput_ReturnsOk()
        {
            var dto = new MessageDto { ReceiverId = "user2", Message = "Hello" };
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "user1") };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };

            _mockMessageRepo.Setup(r => r.Add(It.IsAny<Message>())).Returns(Task.CompletedTask);

            var clientProxyMock = new Mock<IClientProxy>();
            var clientsMock = new Mock<IHubClients>();
            clientsMock.Setup(c => c.User("user2")).Returns(clientProxyMock.Object);
            _mockHubContext.Setup(h => h.Clients).Returns(clientsMock.Object);

            var result = await _controller.SendMessage(dto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteAppli_AuthorizedSender_ReturnsOk()
        {
            var message = new Message { Id = 1, SenderId = "user1", ReceiverId = "user2" };
            _mockMessageRepo.Setup(r => r.GetById(1)).ReturnsAsync(message);
            _mockMessageRepo.Setup(r => r.Delete(1)).Returns(Task.CompletedTask);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "user1") };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };

            var clientProxyMock = new Mock<IClientProxy>();
            var clientsMock = new Mock<IHubClients>();
            clientsMock.Setup(c => c.User("user2")).Returns(clientProxyMock.Object);
            _mockHubContext.Setup(h => h.Clients).Returns(clientsMock.Object);

            var result = await _controller.DeleteAppli(1);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task UpdateMessage_ValidSender_ReturnsOk()
        {
            var userId = "user1";
            var messageId = 1;
            var existingMessage = new Message
            {
                Id = messageId,
                SenderId = userId,
                ReceiverId = "user2",
                Content = "Old content",
                Timestamp = DateTime.UtcNow
            };
            var updateDto = new MessageDto
            {
                Message = "Updated content"
            };

            _mockMessageRepo.Setup(r => r.GetById(messageId)).ReturnsAsync(existingMessage);
            _mockMessageRepo.Setup(r => r.Update(It.IsAny<Message>())).Returns(Task.CompletedTask);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };

            _mockClientProxy
                .Setup(p => p.SendCoreAsync(
                    "UpdateMessage",
                    It.Is<object[]>(args => (int)args[0] == messageId && (string)args[1] == updateDto.Message),
                    default))
                .Returns(Task.CompletedTask);

            var clientsMock = new Mock<IHubClients>();
            clientsMock.Setup(c => c.User("user2")).Returns(_mockClientProxy.Object);
            _mockHubContext.Setup(h => h.Clients).Returns(clientsMock.Object);

            var result = await _controller.UpdateMessage(messageId, updateDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var updated = Assert.IsType<Message>(okResult.Value);
            Assert.Equal("Updated content", updated.Content);
        }

    }
}
