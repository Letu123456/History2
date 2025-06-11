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
using AutoMapper;
using Xunit;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace API.Tests
{
    public class ArtifactControllerTest
    {
        private readonly Mock<IArtifactRepo> _mockRepo = new();
        private readonly FilesService _realFilesService;
        private readonly Mock<IMapper> _mockMapper = new();
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly ArtifactController _controller;

        public ArtifactControllerTest()
        {
            var store = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            var inMemorySettings = new Dictionary<string, string> {
                { "AWS:AccessKey", "FAKE_ACCESS_KEY" },
                { "AWS:SecretKey", "FAKE_SECRET_KEY" }
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _realFilesService = new FilesService(configuration);

            _controller = new ArtifactController(_mockRepo.Object, _realFilesService, _mockMapper.Object, _mockUserManager.Object);
        }

        [Fact]
        public async Task GetAllArtifact_ReturnsOkResult_WithListOfArtifacts()
        {
            var artifacts = new List<Artifact> { new Artifact { ArtifactName = "Artifact A" } };
            _mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(artifacts);

            var result = await _controller.GetAllArtifact();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Artifact>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetArtifactById_ValidId_ReturnsArtifact()
        {
            var artifact = new Artifact { Id = 1, ArtifactName = "Test" };
            _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(artifact);

            var result = await _controller.GetArtifactById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnArtifact = Assert.IsType<Artifact>(okResult.Value);
            Assert.Equal(1, returnArtifact.Id);
        }

        [Fact]
        public async Task DeleteArtifact_ExistingId_ReturnsOk()
        {
            var artifact = new Artifact { Id = 1 };
            _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(artifact);
            _mockRepo.Setup(r => r.Delete(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteArtifact(1);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task UpdateStatusTrue_ExistingArtifact_SetsStatusTrue()
        {
            var artifact = new Artifact { Id = 1, Status = false };
            _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(artifact);
            _mockRepo.Setup(r => r.Update(It.IsAny<Artifact>())).Returns(Task.CompletedTask);

            var result = await _controller.UpdateStatusTrue(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var updated = Assert.IsType<Artifact>(okResult.Value);
            Assert.True(updated.Status);
        }

        [Fact]
        public async Task UpdateStatusFalse_ExistingArtifact_SetsStatusFalse()
        {
            var artifact = new Artifact { Id = 1, Status = true };
            _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(artifact);
            _mockRepo.Setup(r => r.Update(It.IsAny<Artifact>())).Returns(Task.CompletedTask);

            var result = await _controller.UpdateStatusFalse(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var updated = Assert.IsType<Artifact>(okResult.Value);
            Assert.False(updated.Status);
        }
        [Fact]
        public async Task CreateArtifact_ValidInput_ReturnsOk()
        {
            var artifactDto = new ArtifactDto
            {
                Name = "Test Artifact",
                Description = "Test Desc",
                Image = new FormFile(Stream.Null, 0, 0, "Data", "image.jpg"),
                Podcast = new FormFile(Stream.Null, 0, 0, "Data", "podcast.mp3"),
                CategoryArtifactId = 1,
                MuseumId = 1,
                //DateDiscovered = DateTime.Now,
                Dimenson = "10x10x10",
                Weight = "1kg",
                Material = "Gỗ",
                Function = "Trưng bày",
                Condition = "Nguyên vẹn",
                Origin = "Việt Nam"
            };

            var images = new List<IFormFile> {
                new FormFile(Stream.Null, 0, 0, "Data", "image1.jpg")
            };

            _mockRepo.Setup(r => r.Add(It.IsAny<Artifact>())).Returns(Task.CompletedTask);

            var result = await _controller.CreateArtifact(artifactDto, images);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(artifactDto, okResult.Value);
        }

        [Fact]
        public async Task UpdateArtifact_ValidInput_ReturnsOk()
        {
            var artifact = new Artifact
            {
                Id = 1,
                Image = "old.jpg",
                Podcast = "old.mp3",
                Images = new List<ArtifactImage>()
            };
            _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(artifact);
            _mockRepo.Setup(r => r.Update(It.IsAny<Artifact>())).Returns(Task.CompletedTask);

            var updateDto = new ArtifactUpdateDto
            {
                Name = "Updated Name",
                Description = "Updated Desc",
                Image = new FormFile(Stream.Null, 0, 0, "Data", "newimage.jpg"),
                Podcast = new FormFile(Stream.Null, 0, 0, "Data", "newpodcast.mp3"),
                CategoryArtifactId = 2,
                MuseumId = 1,
               // DateDiscovered = DateTime.Now,
                Dimenson = "20x20",
                Function = "Lưu trữ",
                Weight = "2kg",
                Material = "Sắt",
                Condition = "Hư hại",
                Origin = "Trung Quốc"
            };

            var images = new List<IFormFile> {
                new FormFile(Stream.Null, 0, 0, "Data", "img.jpg")
            };

            var result = await _controller.UpdateArtifact(1, updateDto, images);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var updated = Assert.IsType<Artifact>(okResult.Value);
            Assert.Equal("Updated Name", updated.ArtifactName);
        }
    }
}

