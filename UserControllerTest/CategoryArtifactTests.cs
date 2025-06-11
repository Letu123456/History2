using API.Controllers;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace API.Tests
{
    public class CategoryArtifactTests
    {
        private readonly Mock<ICategoryArtifactRepo> _mockRepo = new();
        private readonly CategoryArtifactController _controller;

        public CategoryArtifactTests()
        {
            _controller = new CategoryArtifactController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllCateArtifact_ReturnsOk()
        {
            _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<CategoryArtifact>());
            var result = await _controller.GetAllCateArtifact();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetCateArtifactById_ValidId_ReturnsOk()
        {
            _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(new CategoryArtifact { Id = 1 });
            var result = await _controller.GetCateArtifactById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<CategoryArtifact>(okResult.Value);
        }

        [Fact]
        public async Task CreateCateArtifact_ValidInput_ReturnsOk()
        {
            var result = await _controller.CreateCateArtifact("New Category", 2);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var created = Assert.IsType<CategoryArtifact>(okResult.Value);
            Assert.Equal("New Category", created.Name);
            Assert.Equal(2, created.CategoryHistoricalId);
        }

        [Fact]
        public async Task UpdateCateArtifact_ValidInput_ReturnsOk()
        {
            var existing = new CategoryArtifact { Id = 1, Name = "Old", CategoryHistoricalId = 1 };
            _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.Update(It.IsAny<CategoryArtifact>())).Returns(Task.CompletedTask);

            var result = await _controller.UpdateCateArtifact(1, "Updated", 5);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updated = Assert.IsType<CategoryArtifact>(okResult.Value);
            Assert.Equal("Updated", updated.Name);
            Assert.Equal(5, updated.CategoryHistoricalId);
        }

        [Fact]
        public async Task DeleteCateArtifact_ValidId_ReturnsOk()
        {
            var existing = new CategoryArtifact { Id = 1 };
            _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.Delete(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteCateArtifact(1);
            Assert.IsType<OkResult>(result);
        }
    }
}
