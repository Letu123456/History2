//using API.Controllers;
//using Business.DTO;
//using Business.Model;
//using DataAccess.IRepo;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Xunit;

//namespace API.Tests
//{
//    public class RoleControllerTests
//    {
//        private readonly Mock<IRoleRepo> _mockRoleRepo;
//        private readonly RoleController _controller;

//        public RoleControllerTests()
//        {
//            _mockRoleRepo = new Mock<IRoleRepo>();
//            _controller = new RoleController(_mockRoleRepo.Object);
//        }

//        [Fact]
//        public async Task GetAllRole_ReturnsOk()
//        {
//            var roles = new List<Role> { new Role { Name = "admin" }, new Role { Name = "manager" } };
//            _mockRoleRepo.Setup(r => r.GetRoles()).ReturnsAsync(roles.AsEnumerable());

//            var result = await _controller.GetAllRole();
//            var okResult = Assert.IsType<OkObjectResult>(result);
//            var returnedRoles = Assert.IsAssignableFrom<IEnumerable<Role>>(okResult.Value);
//            Assert.Equal(2, returnedRoles.Count());
//        }

//        [Fact]
//        public async Task CreateRole_ReturnsOk()
//        {
//            // Arrange
//            var mockRoleRepo = new Mock<IRoleRepo>();
//            var controller = new RoleController(mockRoleRepo.Object);

//            mockRoleRepo
//                .Setup(r => r.CreateRole("admin"))
//                .ReturnsAsync("Role created");

//            // Act
//            var result = await controller.CreateRole("admin");

//            // Assert
//            var okResult = Assert.IsType<OkObjectResult>(result);
//            Assert.Equal("Role created", okResult.Value);
//        }



//        [Fact]
//        public async Task DeleteRole_ReturnsOk()
//        {
//            // Arrange
//            var mockRoleRepo = new Mock<IRoleRepo>();
//            var controller = new RoleController(mockRoleRepo.Object);

//            mockRoleRepo
//                .Setup(r => r.DeleteRole("role123"))
//                .ReturnsAsync("Deleted");

//            // Act
//            var result = await controller.DeleteRole("role123");

//            // Assert
//            var okResult = Assert.IsType<OkObjectResult>(result);
//            Assert.Equal("Deleted", okResult.Value);
//        }


//        [Fact]
//        public async Task GetUserRoles_ReturnsOk()
//        {
//            var roles = new List<string> { "admin", "user" };
//            _mockRoleRepo.Setup(r => r.GetUserRole("user1")).ReturnsAsync(roles);

//            var result = await _controller.GetUserRoles("user1");
//            var okResult = Assert.IsType<OkObjectResult>(result);
//            var returned = Assert.IsAssignableFrom<IEnumerable<string>>(okResult.Value);
//            Assert.Contains("admin", returned);
//        }

//        [Fact]
//        public async Task AddUserToRole_ReturnsOk()
//        {
//            _mockRoleRepo.Setup(r => r.AddUserToRole("user1", "admin"))
//                        .ReturnsAsync("User added to role");

//            var controller = new RoleController(_mockRoleRepo.Object);

//            var result = await controller.AddUserToRole("user1", "admin");

//            var okResult = Assert.IsType<OkObjectResult>(result);
//            Assert.Equal("User added to role", okResult.Value);
//        }


//        [Fact]
//        public async Task DeleteUserFromRole_ReturnsOk()
//        {
//            _mockRoleRepo.Setup(r => r.RemoveUserFromRole("user1", "admin")).ReturnsAsync(true);

//            var result = await _controller.DeleteUserFromRole("user1", "admin");
//            var okResult = Assert.IsType<OkObjectResult>(result);
//            Assert.True((bool)okResult.Value);
//        }

//        [Fact]
//        public async Task ChangeUserRole_ReturnsOk()
//        {
//            _mockRoleRepo.Setup(r => r.ChangeUserRole("user1", "admin", "manager")).ReturnsAsync(true);

//            var result = await _controller.ChangeUserRole("user1", "admin", "manager");
//            var okResult = Assert.IsType<OkObjectResult>(result);
//            Assert.True((bool)okResult.Value);
//        }

//        [Fact]
//        public async Task UpdateUserRole_ReturnsOk()
//        {
//            var dto = new UpdateRoleDto { UserId = "user1", NewRole = "manager" };
//            _mockRoleRepo.Setup(r => r.UpdateUserRole("user1", "manager")).ReturnsAsync(true);

//            var result = await _controller.UpdateUserRole(dto);
//            var okResult = Assert.IsType<OkObjectResult>(result);
//            Assert.True((bool)okResult.Value);
//        }

//        [Fact]
//        public async Task UpdateUserRoleToManager_ReturnsOk()
//        {
//            _mockRoleRepo.Setup(r => r.UpdateUserRole("user1", "manager")).ReturnsAsync(true);

//            var result = await _controller.UpdateUserRoleToManager("user1");
//            var okResult = Assert.IsType<OkObjectResult>(result);
//            Assert.True((bool)okResult.Value);
//        }
//    }
//}
//using API.Controllers;
//using Business.DTO;
//using DataAccess.IRepo;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Xunit;

//namespace API.Tests
//{
//    public class RoleControllerTests
//    {
//        private readonly Mock<IRoleRepo> _mockRoleRepo = new();
//        private readonly RoleController _controller;

//        public RoleControllerTests()
//        {
//            _controller = new RoleController(_mockRoleRepo.Object);
//        }

//        [Fact]
//        public async Task GetAllRole_ReturnsOk()
//        {
//            _mockRoleRepo.Setup(r => r.GetRoles())
//                         .ReturnsAsync(new List<string> { "admin", "user" });

//            var result = await _controller.GetAllRole();

//            var okResult = Assert.IsType<OkObjectResult>(result);
//            var roles = Assert.IsAssignableFrom<IEnumerable<string>>(okResult.Value);
//            Assert.Contains("admin", roles);
//        }


//        [Fact]
//        public async Task CreateRole_ReturnsOk()
//        {
//            _mockRoleRepo.Setup(r => r.CreateRole("admin")).ReturnsAsync("Role created");

//            var result = await _controller.CreateRole("admin");

//            var okResult = Assert.IsType<OkObjectResult>(result);
//            Assert.Equal("Role created", okResult.Value);
//        }

//        [Fact]
//        public async Task DeleteRole_ReturnsOk()
//        {
//            _mockRoleRepo.Setup(r => r.DeleteRole("role123")).ReturnsAsync("Role deleted");

//            var result = await _controller.DeleteRole("role123");

//            var okResult = Assert.IsType<OkObjectResult>(result);
//            Assert.Equal("Role deleted", okResult.Value);
//        }

//        [Fact]
//        public async Task AddUserToRole_ReturnsOk()
//        {
//            _mockRoleRepo.Setup(r => r.AddUserToRole("user1", "admin")).ReturnsAsync("User added");

//            var result = await _controller.AddUserToRole("user1", "admin");

//            var okResult = Assert.IsType<OkObjectResult>(result);
//            Assert.Equal("User added", okResult.Value);
//        }

//        [Fact]
//        public async Task RemoveUserFromRole_ReturnsOk()
//        {
//            _mockRoleRepo.Setup(r => r.RemoveUserFromRole("user1", "admin")).ReturnsAsync("User removed");

//            var result = await _controller.DeleteUserFromRole("user1", "admin");

//            var okResult = Assert.IsType<OkObjectResult>(result);
//            Assert.Equal("User removed", okResult.Value);
//        }

//        [Fact]
//        public async Task ChangeUserRole_ReturnsOk()
//        {
//            _mockRoleRepo.Setup(r => r.ChangeUserRole("user1", "admin", "manager")).ReturnsAsync("Role changed");

//            var result = await _controller.ChangeUserRole("user1", "admin", "manager");

//            var okResult = Assert.IsType<OkObjectResult>(result);
//            Assert.Equal("Role changed", okResult.Value);
//        }

//        [Fact]
//        public async Task UpdateUserRole_ReturnsOk()
//        {
//            var dto = new UpdateRoleDto { UserId = "user1", NewRole = "editor" };
//            _mockRoleRepo.Setup(r => r.UpdateUserRole("user1", "editor")).ReturnsAsync("Updated");

//            var result = await _controller.UpdateUserRole(dto);

//            var okResult = Assert.IsType<OkObjectResult>(result);
//            Assert.Equal("Updated", okResult.Value);
//        }

//        [Fact]
//        public async Task UpdateUserRoleToManager_ReturnsOk()
//        {
//            _mockRoleRepo.Setup(r => r.UpdateUserRole("user1", "manager")).ReturnsAsync("Updated to manager");

//            var result = await _controller.UpdateUserRoleToManager("user1");

//            var okResult = Assert.IsType<OkObjectResult>(result);
//            Assert.Equal("Updated to manager", okResult.Value);
//        }

//        [Fact]
//        public async Task GetUserRoles_ReturnsOk()
//        {
//            _mockRoleRepo.Setup(r => r.GetUserRole("user1")).ReturnsAsync(new List<string> { "user", "admin" });

//            var result = await _controller.GetUserRoles("user1");

//            var okResult = Assert.IsType<OkObjectResult>(result);
//            var roles = Assert.IsType<List<string>>(okResult.Value);
//            Assert.Contains("admin", roles);
//        }
//    }
//}
using API.Controllers;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Business.DTO;
using Business.Model;

namespace API.Tests
{
    public class RoleControllerTests
    {
        private readonly Mock<IRoleRepo> _mockRoleRepo;
        private readonly RoleController _controller;

        public RoleControllerTests()
        {
            _mockRoleRepo = new Mock<IRoleRepo>();
            _controller = new RoleController(_mockRoleRepo.Object);
        }

        [Fact]
        public async Task GetAllRole_ReturnsOk()
        {
            // Arrange
            var roles = new List<Role>
    {
        new Role { Name = "admin" },
        new Role { Name = "user" }
    };

            _mockRoleRepo.Setup(r => r.GetRoles()).ReturnsAsync(roles);

            var controller = new RoleController(_mockRoleRepo.Object);

            // Act
            var result = await controller.GetAllRole();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnRoles = Assert.IsAssignableFrom<IEnumerable<Role>>(okResult.Value);
            Assert.Equal(2, returnRoles.Count());
        }


        [Fact]
        public async Task CreateRole_ReturnsOk()
        {
            _mockRoleRepo.Setup(r => r.CreateRole("admin"))
                         .ReturnsAsync("Success");

            var result = await _controller.CreateRole("admin");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Success", okResult.Value);
        }

        [Fact]
        public async Task DeleteRole_ReturnsOk()
        {
            _mockRoleRepo.Setup(r => r.DeleteRole("role123"))
                         .ReturnsAsync("Deleted");

            var result = await _controller.DeleteRole("role123");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Deleted", okResult.Value);
        }

        [Fact]
        public async Task AddUserToRole_ReturnsOk()
        {
            _mockRoleRepo.Setup(r => r.AddUserToRole("user1", "admin"))
                         .ReturnsAsync("Added");

            var result = await _controller.AddUserToRole("user1", "admin");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Added", okResult.Value);
        }

        [Fact]
        public async Task RemoveUserFromRole_ReturnsOk()
        {
            _mockRoleRepo.Setup(r => r.RemoveUserFromRole("user1", "admin"))
                         .ReturnsAsync("Removed");

            var result = await _controller.DeleteUserFromRole("user1", "admin");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Removed", okResult.Value);
        }

        [Fact]
        public async Task ChangeUserRole_ReturnsOk()
        {
            _mockRoleRepo.Setup(r => r.ChangeUserRole("user1", "admin", "manager"))
                         .ReturnsAsync("Changed");

            var result = await _controller.ChangeUserRole("user1", "admin", "manager");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Changed", okResult.Value);
        }

        [Fact]
        public async Task UpdateUserRole_ReturnsOk()
        {
            var dto = new UpdateRoleDto
            {
                UserId = "user1",
                NewRole = "manager"
            };

            _mockRoleRepo.Setup(r => r.UpdateUserRole(dto.UserId, dto.NewRole))
                         .ReturnsAsync("Updated");

            var result = await _controller.UpdateUserRole(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Updated", okResult.Value);
        }

        [Fact]
        public async Task UpdateUserRoleToManager_ReturnsOk()
        {
            _mockRoleRepo.Setup(r => r.UpdateUserRole("user1", "manager"))
                         .ReturnsAsync("ManagerUpdated");

            var result = await _controller.UpdateUserRoleToManager("user1");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("ManagerUpdated", okResult.Value);
        }

        [Fact]
        public async Task GetUserRoles_ReturnsOk()
        {
            _mockRoleRepo.Setup(r => r.GetUserRole("user1"))
                         .ReturnsAsync(new List<string> { "admin", "editor" });

            var result = await _controller.GetUserRoles("user1");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var roles = Assert.IsAssignableFrom<IEnumerable<string>>(okResult.Value);
            Assert.Contains("admin", roles);
        }
    }
}

