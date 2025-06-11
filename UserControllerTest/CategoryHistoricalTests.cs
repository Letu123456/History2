using API.Controllers;
using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace API.Tests
{
    public class CategoryHistoricalTests
    {
        private readonly Mock<ICategoryHistoricalRepo> _mockRepo = new();
        private readonly CategoryHistoricalController _controller;

        public CategoryHistoricalTests()
        {
            _controller = new CategoryHistoricalController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllCateHistory_ReturnsOk()
        {
            _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<CategoryHistorical>());
            var result = await _controller.GetAllCateHistory();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetHistoricalById_ValidId_ReturnsOk()
        {
            _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(new CategoryHistorical { Id = 1 });
            var result = await _controller.GetHistoricalById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<CategoryHistorical>(okResult.Value);
        }

        [Fact]
        public async Task CreateHistorical_ValidInput_ReturnsOk()
        {
            var dto = new CategoryHistoryDto { Name = "Lịch sử hiện đại" };
            _mockRepo.Setup(r => r.Add(It.IsAny<CategoryHistorical>())).Returns(Task.CompletedTask);

            var result = await _controller.CreateHistorical(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var created = Assert.IsType<CategoryHistorical>(okResult.Value);
            Assert.Equal("Lịch sử hiện đại", created.Name);
        }

        [Fact]
        public async Task UpdateHistorical_ValidInput_ReturnsOk()
        {
            var existing = new CategoryHistorical { Id = 1, Name = "Cũ" };
            var update = new CategoryHistorical { Id = 1, Name = "Mới" };

            _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.Update(update)).Returns(Task.CompletedTask);

            var result = await _controller.UpdateHistorical(1, update);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updated = Assert.IsType<CategoryHistorical>(okResult.Value);
            Assert.Equal("Mới", updated.Name);
        }

        [Fact]
        public async Task DeleteCateHistorical_ValidId_ReturnsOk()
        {
            var history = new CategoryHistorical { Id = 1 };
            _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(history);
            _mockRepo.Setup(r => r.Delete(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteCateHistorical(1);
            Assert.IsType<OkResult>(result);
        }
    }
}
