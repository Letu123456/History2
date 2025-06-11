using Xunit;
using Moq;
using API.Controllers;
using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public class UserControllerTests
{
    private readonly Mock<IUserRepo> _mockUserRepo;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mockUserRepo = new Mock<IUserRepo>();
        _controller = new UserController(_mockUserRepo.Object, null, null, null, null);
    }

    [Fact]
    public async Task Login_ReturnsOkResult_WithTokenAndRole()
    {
        var loginDto = new LoginDto { Email = "user@example.com", Password = "password" };
        var fakeToken = "jwt.token.string";
        var fakeRole = "User";
        var fakeUser = new User { Id = "123", Email = "user@example.com" };

        _mockUserRepo.Setup(repo => repo.LoginAsync(loginDto)).ReturnsAsync(fakeToken);
        _mockUserRepo.Setup(repo => repo.GetUserByEmailAsync(loginDto.Email)).ReturnsAsync(fakeUser);
        _mockUserRepo.Setup(repo => repo.GetUserRoleAsync(fakeUser.Id)).ReturnsAsync(fakeRole);

        var result = await _controller.Login(loginDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<Dictionary<string, string>>(okResult.Value);

        Assert.Equal(fakeToken, response["token"]);
        Assert.Equal(fakeRole, response["role"]);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenTokenIsNull()
    {
        var loginDto = new LoginDto { Email = "invalid@example.com", Password = "wrongpass" };

        _mockUserRepo.Setup(repo => repo.LoginAsync(loginDto)).ReturnsAsync((string)null);

        var result = await _controller.Login(loginDto);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Contains("Email hoặc mật khẩu không đúng", unauthorizedResult.Value.ToString());
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenUserNotFound()
    {
        var loginDto = new LoginDto { Email = "notfound@example.com", Password = "123456" };
        var fakeToken = "jwt.token.string";

        _mockUserRepo.Setup(repo => repo.LoginAsync(loginDto)).ReturnsAsync(fakeToken);
        _mockUserRepo.Setup(repo => repo.GetUserByEmailAsync(loginDto.Email)).ReturnsAsync((User)null);

        var result = await _controller.Login(loginDto);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Contains("Không tìm thấy người dùng", unauthorizedResult.Value.ToString());
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenUserHasNoRole()
    {
        var loginDto = new LoginDto { Email = "user@example.com", Password = "123456" };
        var fakeToken = "jwt.token.string";
        var fakeUser = new User { Id = "123", Email = "user@example.com" };

        _mockUserRepo.Setup(repo => repo.LoginAsync(loginDto)).ReturnsAsync(fakeToken);
        _mockUserRepo.Setup(repo => repo.GetUserByEmailAsync(loginDto.Email)).ReturnsAsync(fakeUser);
        _mockUserRepo.Setup(repo => repo.GetUserRoleAsync(fakeUser.Id)).ReturnsAsync((string)null);

        var result = await _controller.Login(loginDto);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Contains("Người dùng chưa có vai trò", unauthorizedResult.Value.ToString());
    }

    [Fact]
    public async Task Register_ReturnsOkResult_WhenRegistrationIsSuccessful()
    {
        var registerDto = new RegisterDto { Email = "test@example.com", Password = "Test123!" };
        _mockUserRepo.Setup(repo => repo.RegisterAsync(registerDto))
                     .ReturnsAsync("Registration successful");

        var result = await _controller.Register(registerDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<Dictionary<string, string>>(okResult.Value);
        Assert.Equal("Registration successful", response["message"]);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var registerDto = new RegisterDto { Email = "test@example.com", Password = "Test123!" };
        _mockUserRepo.Setup(repo => repo.RegisterAsync(registerDto))
                     .ThrowsAsync(new Exception("Email already exists"));

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, objectResult.StatusCode);  // StatusCode should be 400 for BadRequest

        var response = Assert.IsType<Dictionary<string, string>>(objectResult.Value);
        Assert.Equal("Email already exists", response["message"]);
    }


}
