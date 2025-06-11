using API.Controllers;
using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using DataAccess.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using API.Hubs;

namespace API.Tests
{
    public class BlogControllerTests
    {
        private readonly Mock<IBlogRepo> _mockBlogRepo = new();
        private readonly FilesService _realFilesService;
        private readonly GPTService _realGptService;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<INotificationRepo> _mockNotificationRepo = new();
        private readonly Mock<IHubContext<ChatHub>> _mockHubContext = new();
        private readonly BlogController _controller;

        public BlogControllerTests()
        {
            var store = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            var inMemorySettings = new Dictionary<string, string> {
                { "AWS:AccessKey", "FAKE_ACCESS_KEY" },
                { "AWS:SecretKey", "FAKE_SECRET_KEY" },
                { "OpenAI:ApiKey", "FAKE_API_KEY" }
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _realFilesService = new FilesService(configuration);
            _realGptService = new GPTService(configuration);

            _controller = new BlogController(
                _mockBlogRepo.Object,
                _realFilesService,
                _mockUserManager.Object,
                _mockNotificationRepo.Object,
                _realGptService,
                _mockHubContext.Object
            );
        }

        [Fact]
        public async Task GetAllEvent_ReturnsOk()
        {
            _mockBlogRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Blog>());
            var result = await _controller.GetAllEvent();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetEventById_ValidId_ReturnsOk()
        {
            _mockBlogRepo.Setup(r => r.GetById(1)).ReturnsAsync(new Blog { Id = 1 });
            var result = await _controller.GetEventById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var blog = Assert.IsType<Blog>(okResult.Value);
            Assert.Equal(1, blog.Id);
        }

        [Fact]
        public async Task GetAllBlogByHashtag_Valid_ReturnsOk()
        {
            _mockBlogRepo.Setup(r => r.GetAllByHashtag("test")).ReturnsAsync(new List<Blog>());
            var result = await _controller.GetAllBlogByHashtag("test");
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteEvent_ExistingBlog_ReturnsOk()
        {
            var blog = new Blog { Id = 1 };
            _mockBlogRepo.Setup(r => r.GetById(1)).ReturnsAsync(blog);
            _mockBlogRepo.Setup(r => r.Delete(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteEvent(1);
            Assert.IsType<OkResult>(result);
        }
        [Fact]
        public async Task CreateEvent_ValidBlog_ReturnsOk()
        {
            var blogDto = new BlogDto
            {
                Title = "Test Title",
                Content = "Test Content",
                Image = new FormFile(Stream.Null, 0, 0, "Data", "image.jpg"),
                CategoryBlogId = 1
            };
            var hashtags = new List<string> { "history", "culture" };

            var user = new User { Id = "user1", Point = 0, UserName = "Tester" };
            var userId = "user1";

            _mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);
            _mockBlogRepo.Setup(r => r.Add(It.IsAny<Blog>())).Returns(Task.CompletedTask);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            var result = await _controller.CreateEvent(blogDto, hashtags);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateEvent_ValidBlog_ReturnsOk()
        {
            var blog = new Blog
            {
                Id = 1,
                UserId = "user1",
                Title = "Old Title",
                Image = "old.jpg",
                HastagOfBlog = new List<HastagOfBlog>()
            };
            var blogDto = new BlogDto
            {
                Title = "Updated Title",
                Content = "Updated Content",
                Image = new FormFile(Stream.Null, 0, 0, "Data", "new.jpg"),
                CategoryBlogId = 2
            };
            var hashtags = new List<string> { "updated" };

            _mockBlogRepo.Setup(r => r.GetById(1)).ReturnsAsync(blog);
            _mockBlogRepo.Setup(r => r.Update(It.IsAny<Blog>())).Returns(Task.CompletedTask);

            var result = await _controller.UpdateEvent(1, blogDto, hashtags);
            Assert.IsType<OkObjectResult>(result);
        }



        
    }
}
