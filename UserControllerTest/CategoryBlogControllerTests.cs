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
    public class CategoryBlogControllerTests
    {
        private readonly Mock<ICategoryBlogRepo> _mockRepo = new();
        private readonly CategoryBlogController _controller;

        public CategoryBlogControllerTests()
        {
            _controller = new CategoryBlogController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllCateArtifact_ReturnsOk()
        {
            _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<CategoryBlog>());
            var result = await _controller.GetAllCateArtifact();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetCateArtifactById_ValidId_ReturnsOk()
        {
            _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(new CategoryBlog { Id = 1 });
            var result = await _controller.GetCateArtifactById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<CategoryBlog>(okResult.Value);
        }

        [Fact]
        public async Task CreateCateArtifact_ValidInput_ReturnsOk()
        {
            var dto = new CategoryHistoryDto { Name = "Lịch sử hiện đại" };
            _mockRepo.Setup(r => r.Add(It.IsAny<CategoryBlog>())).Returns(Task.CompletedTask);

            var result = await _controller.CreateCateArtifact(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var created = Assert.IsType<CategoryBlog>(okResult.Value);
            Assert.Equal("Lịch sử hiện đại", created.Name);
        }

        [Fact]
        public async Task UpdateCateArtifact_ValidInput_ReturnsOk()
        {
            var existing = new CategoryBlog { Id = 1, Name = "Cũ" };
            var dto = new CategoryHistoryDto { Name = "Mới" };
            _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.Update(It.IsAny<CategoryBlog>())).Returns(Task.CompletedTask);

            var result = await _controller.UpdateCateArtifact(1, dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updated = Assert.IsType<CategoryBlog>(okResult.Value);
            Assert.Equal("Mới", updated.Name);
        }

        [Fact]
        public async Task DeleteCateArtifact_ValidId_ReturnsOk()
        {
            var existing = new CategoryBlog { Id = 1 };
            _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.Delete(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteCateArtifact(1);
            Assert.IsType<OkResult>(result);
        }
    }
}
