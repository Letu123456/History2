using API.Controllers;
using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace API.Tests
{
    public class AnswerControllerTests
    {
        private readonly Mock<IAnswerRepo> _mockAnswerRepo = new();
        private readonly AnswerController _controller;

        public AnswerControllerTests()
        {
            _controller = new AnswerController(_mockAnswerRepo.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            _mockAnswerRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Answer>());
            var result = await _controller.GetAll();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsOk()
        {
            _mockAnswerRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Answer { Id = 1 });
            var result = await _controller.GetById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<Answer>(okResult.Value);
        }

        [Fact]
        public async Task Create_ValidInput_ReturnsCreated()
        {
            var dto = new AnswerDto { QuestionId = 1, Text = "Answer", IsCorrect = true };

            _mockAnswerRepo.Setup(r => r.AddAsync(It.IsAny<Answer>())).Returns(Task.CompletedTask);

            _controller.ModelState.Clear();
            var result = await _controller.Create(dto);
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Update_ValidInput_ReturnsNoContent()
        {
            var dto = new AnswerDto { QuestionId = 1, Text = "Updated Answer", IsCorrect = false };
            var existing = new Answer { Id = 1 };

            _mockAnswerRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockAnswerRepo.Setup(r => r.UpdateAsync(It.IsAny<Answer>())).Returns(Task.CompletedTask);

            _controller.ModelState.Clear();
            var result = await _controller.Update(1, dto);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ValidId_ReturnsNoContent()
        {
            _mockAnswerRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);
            var result = await _controller.Delete(1);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
