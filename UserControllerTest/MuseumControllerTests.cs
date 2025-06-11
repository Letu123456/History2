using API.Controllers;
using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using DataAccess.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using System;

namespace API.Tests
{
    public class MuseumControllerTests
    {
        private readonly Mock<IMuseumRepo> _mockMuseumRepo = new();
        private readonly FilesService _realFilesService;
        private readonly MuseumController _controller;

        public MuseumControllerTests()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "AWS:AccessKey", "FAKE" },
                    { "AWS:SecretKey", "FAKE" }
                }).Build();

            _realFilesService = new FilesService(config);
            _controller = new MuseumController(_mockMuseumRepo.Object, _realFilesService);
        }

        [Fact]
        public async Task GetAllMuseum_ReturnsOk()
        {
            _mockMuseumRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Museum>());
            var result = await _controller.GetAllMuseum();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetMuseumById_ValidId_ReturnsOk()
        {
            _mockMuseumRepo.Setup(r => r.GetById(1)).ReturnsAsync(new Museum { Id = 1 });
            var result = await _controller.GetMuseumById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<Museum>(okResult.Value);
        }

        [Fact]
        public async Task DeleteMuseum_ValidId_ReturnsOk()
        {
            _mockMuseumRepo.Setup(r => r.GetById(1)).ReturnsAsync(new Museum { Id = 1 });
            _mockMuseumRepo.Setup(r => r.Delete(1)).Returns(Task.CompletedTask);
            var result = await _controller.DeleteEvent(1);
            Assert.IsType<OkResult>(result);
        }
        [Fact]
        public async Task CreateMuseum_ValidInput_ReturnsOk()
        {
            var dto = new MuseumDto
            {
                Name = "Museum A",
                Description = "Mô tả",
                Image = new FormFile(Stream.Null, 0, 0, "Data", "img.jpg"),
                Video = new FormFile(Stream.Null, 0, 0, "Data", "vid.mp4"),
                Location = "Hà Nội",
                EstablishYear = "2000", // string
                Contact = "0123456789"
            };

            var images = new List<IFormFile>
            {
                new FormFile(Stream.Null, 0, 0, "Data", "gallery1.jpg"),
                new FormFile(Stream.Null, 0, 0, "Data", "gallery2.jpg")
            };

            _mockMuseumRepo.Setup(r => r.Add(It.IsAny<Museum>())).Returns(Task.CompletedTask);

            _controller.ModelState.Clear();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.CreateMuseum(dto, images);

            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public async Task UpdateMuseum_ValidInput_ReturnsOk()
        {
            var dto = new MuseumDto
            {
                Name = "Updated Museum",
                Description = "New Desc",
                Image = new FormFile(Stream.Null, 0, 0, "Data", "new.jpg"),
                Video = new FormFile(Stream.Null, 0, 0, "Data", "newvid.mp4"),
                Location = "HCM",
                EstablishYear = "2022",
                Contact = "0999888777"
            };

            var images = new List<IFormFile>
            {
                new FormFile(Stream.Null, 0, 0, "Data", "img1.jpg")
            };

            var existing = new Museum
            {
                Id = 1,
                Name = "Old",
                Description = "Old Desc",
                Image = "oldimg.jpg",
                Video = "oldvid.mp4",
                Images = new List<MuseumImage>()
            };

            _mockMuseumRepo.Setup(r => r.GetById(1)).ReturnsAsync(existing);
            _mockMuseumRepo.Setup(r => r.Update(It.IsAny<Museum>())).Returns(Task.CompletedTask);

            _controller.ModelState.Clear();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.UpdateMuseum(1, dto, images);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
