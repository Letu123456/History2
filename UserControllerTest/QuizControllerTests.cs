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
using System.Threading.Tasks;
using Xunit;
using System;

namespace API.Tests
{
    public class QuizControllerTests
    {
        private readonly Mock<IQuizRepo> _mockQuizRepo = new();
        private readonly Mock<IQuestionRepo> _mockQuestionRepo = new();
        private readonly Mock<IAnswerRepo> _mockAnswerRepo = new();
        private readonly FilesService _realFilesService;
        private readonly QuizController _controller;

        public QuizControllerTests()
        {
            var inMemorySettings = new Dictionary<string, string> {
                { "AWS:AccessKey", "FAKE_ACCESS_KEY" },
                { "AWS:SecretKey", "FAKE_SECRET_KEY" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _realFilesService = new FilesService(configuration);

            _controller = new QuizController(
                _mockQuizRepo.Object,
                _mockQuestionRepo.Object,
                _mockAnswerRepo.Object,
                null, // AppDbContext not needed for these tests
                _realFilesService
            );
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            _mockQuizRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Quiz>());
            var result = await _controller.GetAll();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ValidId_ReturnsOk()
        {
            _mockQuizRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Quiz { Id = 1 });
            var result = await _controller.GetById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<Quiz>(okResult.Value);
        }

        [Fact]
        public async Task CreateQuiz_ValidInput_ReturnsOk()
        {
            var dto = new QuizDto
            {
                Title = "Test Quiz",
                Description = "Test Desc",
                TimeLimit = 60,
                Level = 1,
                IsActive = true,
                CategoryHistoricalId = 1
            };

            _mockQuizRepo.Setup(r => r.AddAsync(It.IsAny<Quiz>())).Returns(Task.CompletedTask);

            var result = await _controller.CreateQuiz(dto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateQuiz_ValidInput_ReturnsOk()
        {
            var dto = new QuizDto
            {
                Title = "Updated Quiz",
                Description = "Updated Desc",
                TimeLimit = 90,
                Level = 2,
                IsActive = false,
                CategoryHistoricalId = 1
            };
            var existing = new Quiz { Id = 1 };

            _mockQuizRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockQuizRepo.Setup(r => r.UpdateAsync(It.IsAny<Quiz>())).Returns(Task.CompletedTask);

            var result = await _controller.UpdateQuiz(1, dto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteQuiz_ValidId_ReturnsOk()
        {
            // Skip complex deletion logic with db context for now
            var result = await _controller.Delete(1);
            Assert.IsType<OkResult>(result); // may return NotFound in real
        }
    }
}
