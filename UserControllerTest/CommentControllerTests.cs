using API.Controllers;
using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using DataAccess.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace API.Tests
{
    public class FakeGPTService : GPTService
    {
        public FakeGPTService() : base(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>()).Build()) { }

        public new Task<bool> IsBlogContentAppropriateAsync(string content)
        {
            return Task.FromResult(true);
        }
    }

    public class CommentControllerTests
    {
        private readonly Mock<ICommentRepo> _mockCommentRepo = new();
        private readonly FakeGPTService _fakeGptService = new();
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly CommentController _controller;

        public CommentControllerTests()
        {
            var store = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object, contextAccessor.Object, claimsFactory.Object, null, null, null, null);

            _controller = new CommentController(
                _mockCommentRepo.Object,
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _fakeGptService
            );
        }

        [Fact]
        public async Task GetAllComment_ReturnsOk()
        {
            _mockCommentRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Comment>());
            var result = await _controller.GetAllComment();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetCommentById_ValidId_ReturnsOk()
        {
            _mockCommentRepo.Setup(r => r.GetById(1)).ReturnsAsync(new Comment { Id = 1 });
            var result = await _controller.GetCommentById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var comment = Assert.IsType<Comment>(okResult.Value);
            Assert.Equal(1, comment.Id);
        }

        [Fact]
        public async Task GetAllCommentByEventId_ReturnsOk()
        {
            _mockCommentRepo.Setup(r => r.GetAllByEventId(10)).ReturnsAsync(new List<Comment>());
            var result = await _controller.GetAllCommentByEventId(10);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteComment_ExistingComment_ReturnsOk()
        {
            var comment = new Comment { Id = 1 };
            _mockCommentRepo.Setup(r => r.GetById(1)).ReturnsAsync(comment);
            _mockCommentRepo.Setup(r => r.Delete(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteComment(1);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task CreateComment_ValidInput_ReturnsOk()
        {
            var commentDto = new CommentDto
            {
                Content = "This is a valid comment.",
                Rating = 5,
                EventId = 1
            };

            var user = new User { Id = "user1", UserName = "TestUser" };
            var userId = "user1";

            _mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
            _mockCommentRepo.Setup(r => r.Add(It.IsAny<Comment>())).Returns(Task.CompletedTask);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims)) }
            };

            var result = await _controller.CreateComment(commentDto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateComment_ValidInput_ReturnsOk()
        {
            var existingComment = new Comment
            {
                Id = 1,
                Content = "Old Content",
                Rating = 3,
                EventId = 1,
                UserId = "user1"
            };

            var commentDto = new CommentDto
            {
                Content = "Updated Content",
                Rating = 4,
                EventId = 2
            };

            var user = new User { Id = "user1", UserName = "TestUser" };

            _mockCommentRepo.Setup(r => r.GetById(1)).ReturnsAsync(existingComment);
            _mockUserManager.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync(user);
            _mockCommentRepo.Setup(r => r.Update(It.IsAny<Comment>())).Returns(Task.CompletedTask);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "user1") };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims)) }
            };

            var result = await _controller.UpdateComment(1, commentDto);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
