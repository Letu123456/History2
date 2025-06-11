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
    public class QuestionControllerTests
    {
        private readonly Mock<IQuestionRepo> _mockQuestionRepo = new();
        private readonly FilesService _realFilesService;
        private readonly QuestionController _controller;

        public QuestionControllerTests()
        {
            var inMemorySettings = new Dictionary<string, string> {
                { "AWS:AccessKey", "FAKE_ACCESS_KEY" },
                { "AWS:SecretKey", "FAKE_SECRET_KEY" }
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _realFilesService = new FilesService(configuration);
            _controller = new QuestionController(_mockQuestionRepo.Object, _realFilesService);
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            _mockQuestionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Question>());
            var result = await _controller.GetAll();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ValidId_ReturnsOk()
        {
            _mockQuestionRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Question { Id = 1 });
            var result = await _controller.GetById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<Question>(okResult.Value);
        }

        [Fact]
        public async Task Create_ValidInput_ReturnsCreated()
        {
            var dto = new QuestionDto
            {
                QuizId = 1,
                Text = "What is the capital of Vietnam?",
                Points = 1,
                Image = new FormFile(Stream.Null, 0, 0, "Data", "image.jpg")
            };

            _mockQuestionRepo.Setup(r => r.AddAsync(It.IsAny<Question>())).Returns(Task.CompletedTask);

            _controller.ModelState.Clear();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.Create(dto);
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Update_ValidInput_ReturnsNoContent()
        {
            var dto = new QuestionDto
            {
                QuizId = 1,
                Text = "Updated Question",
                Points = 2,
                Image = new FormFile(Stream.Null, 0, 0, "Data", "updated.jpg")
            };
            var existing = new Question { Id = 1 };

            _mockQuestionRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockQuestionRepo.Setup(r => r.UpdateAsync(It.IsAny<Question>())).Returns(Task.CompletedTask);

            _controller.ModelState.Clear();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.Update(1, dto);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ValidId_ReturnsNoContent()
        {
            _mockQuestionRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);
            var result = await _controller.Delete(1);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
