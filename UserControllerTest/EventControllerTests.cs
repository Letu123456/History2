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
    public class EventControllerTests
    {
        private readonly Mock<IEventRepo> _mockEventRepo = new();
        private readonly FilesService _realFilesService;
        private readonly Mock<UserManager<User>> _mockUserManager = new();
        private readonly EventController _controller;

        public EventControllerTests()
        {
            var inMemorySettings = new Dictionary<string, string> {
                { "AWS:AccessKey", "FAKE_ACCESS_KEY" },
                { "AWS:SecretKey", "FAKE_SECRET_KEY" }
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            _realFilesService = new FilesService(configuration);

            var store = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            _controller = new EventController(_mockEventRepo.Object, _realFilesService, _mockUserManager.Object);
        }

        [Fact]
        public async Task GetAllEvent_ReturnsOk()
        {
            _mockEventRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Event>());
            var result = await _controller.GetAllEvent();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetEventById_ValidId_ReturnsOk()
        {
            _mockEventRepo.Setup(r => r.GetById(1)).ReturnsAsync(new Event { Id = 1 });
            var result = await _controller.GetEventById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var ev = Assert.IsType<Event>(okResult.Value);
            Assert.Equal(1, ev.Id);
        }

        [Fact]
        public async Task GetAllEventByHashtag_Valid_ReturnsOk()
        {
            _mockEventRepo.Setup(r => r.GetAllByHashtag("test")).ReturnsAsync(new List<Event>());
            var result = await _controller.GetAllEventByHashtag("test");
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteEvent_ExistingEvent_ReturnsOk()
        {
            var ev = new Event { Id = 1 };
            _mockEventRepo.Setup(r => r.GetById(1)).ReturnsAsync(ev);
            _mockEventRepo.Setup(r => r.Delete(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteEvent(1);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task CreateEvent_ValidInput_ReturnsOk()
        {
            var dto = new EventDto
            {
                Name = "Event A",
                Description = "Mô tả",
                Image = new FormFile(Stream.Null, 0, 0, "Data", "image.jpg"),
                Location = "Hà Nội",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                MuseumId = 1
            };

            // ✅ Dùng List thay vì IEnumerable
            var hashtags = new List<string> { "lichsu" };

            // ✅ Giả lập thành công Repo
            _mockEventRepo.Setup(r => r.Add(It.IsAny<Event>())).Returns(Task.CompletedTask);

            // ✅ Làm sạch ModelState
            _controller.ModelState.Clear();

            // ✅ Tạo ControllerContext có HttpContext
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.CreateEvent(dto, hashtags);

            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public async Task UpdateEvent_ValidInput_ReturnsOk()
        {
            var dto = new EventDto
            {
                Name = "Sửa tên",
                Description = "Sửa mô tả",
                Image = new FormFile(Stream.Null, 0, 0, "Data", "new.jpg"),
                Location = "Đà Nẵng",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(2),
                MuseumId = 2
            };
            var hashtags = new List<string> { "update" };

            var ev = new Event
            {
                Id = 1,
                Name = "Cũ",
                Description = "Cũ",
                Image = "old.jpg",
                Hastag = new List<Hastag>()
            };

            _mockEventRepo.Setup(r => r.GetById(1)).ReturnsAsync(ev);
            _mockEventRepo.Setup(r => r.Update(It.IsAny<Event>())).Returns(Task.CompletedTask);

            _controller.ModelState.Clear();

            var result = await _controller.UpdateEvent(1, dto, hashtags);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetAllByMuseumId_UserHasMuseum_ReturnsOk()
        {
            var user = new User { Id = "abc", MuseumId = 5 };
            _mockUserManager.Setup(m => m.FindByIdAsync("abc")).ReturnsAsync(user);
            _mockEventRepo.Setup(r => r.GetAllByMuseumId(5)).ReturnsAsync(new List<Event>());

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "abc") };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims)) }
            };

            var result = await _controller.GetAllByMuseumId();
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
