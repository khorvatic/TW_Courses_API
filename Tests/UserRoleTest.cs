using Application.DTO.UserRole;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Presentation.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    public class UserRoleTest
    {
        [Fact]
        public async Task GetUserRoleByCompositeId_Should_ReturnOk_WhenUserRoleExists()
        {
            // Arrange
            var mockService = new Mock<IUserRoleService>();
            mockService.Setup(service => service.GetByCompositeIdAsync(1, 1))
                .ReturnsAsync(new UserRoleDto { UserId = 1, RoleId = 1 });

            // Act
            var controller = new UserRoleController(mockService.Object);
            var result = await controller.GetUserRoleByCompositeId(1, 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var userRole = Assert.IsType<UserRoleDto>(okResult.Value);
            Assert.Equal(1, userRole.RoleId);
            Assert.Equal(1, userRole.UserId);
        }

        [Fact]
        public async Task GetUserRoleByCompositeId_Should_ThrowNotFoundException_WhenUserRoleDoesntExist()
        {
            // Arrange
            var mockService = new Mock<IUserRoleService>();
            var userId = 999;
            var roleId = 888;
            mockService.Setup(service => service.GetByCompositeIdAsync(userId, roleId))
                .ThrowsAsync(new NotFoundException("UserRole not found"));

            // Act
            var controller = new UserRoleController(mockService.Object);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() 
                => controller.GetUserRoleByCompositeId(userId, roleId));
        }

        [Fact]
        public async Task AssignRoleToUser_Should_ReturnCreatedAtAction_WhenRoleIsAssigned()
        {
            // Arrange
            var mockService = new Mock<IUserRoleService>();
            var userId = 1;
            var roleId = 2;
            var createUserRole = new UpdateUserRoleDto { RoleId = roleId, UserId = userId };
            mockService.Setup(service => service.CreateUserRoleAsync(createUserRole))
                .ReturnsAsync(new UserRoleDto { UserId = userId, RoleId = roleId });

            // Act
            var controller = new UserRoleController(mockService.Object);
            var result = await controller.AssignRoleToUser(createUserRole);

            // Assert
            var createdAtAction = Assert.IsType<CreatedAtActionResult>(result.Result);
            var userRole = Assert.IsType<UserRoleDto>(createdAtAction.Value);
            Assert.Equal(nameof(UserRoleController.GetUserRoleByCompositeId), createdAtAction.ActionName);
            Assert.Equal(userId, createdAtAction.RouteValues["userId"]);
            Assert.Equal(roleId, createdAtAction.RouteValues["roleId"]);
        }

        [Fact]
        public async Task GetAllUserRoles_Should_ReturnOk_WithList()
        {
            // Arrange
            var mockService = new Mock<IUserRoleService>();
            var expectedUserRoles = new List<UserRoleDto> {
                new UserRoleDto { RoleId = 1, UserId = 1 },
                new UserRoleDto { RoleId = 2, UserId = 2 }
            };
            mockService.Setup(service => service.GetAllUserRolesAsync())
                .ReturnsAsync(expectedUserRoles);

            // Act
            var controller = new UserRoleController(mockService.Object);
            var result = await controller.GetAllUserRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var userRoles = Assert.IsType<List<UserRoleDto>>(okResult.Value);
            Assert.Equal(expectedUserRoles.Count, userRoles.Count);
        }

        [Fact]
        public async Task GetRolesByUserId_Should_ReturnOk_WhenUserExists()
        {
            // Arrange
            var mockService = new Mock<IUserRoleService>();
            var userId = 1;
            var expectedRoles = new List<UserRoleDto> {
                new UserRoleDto { UserId = userId, RoleId = 1 },
                new UserRoleDto { UserId = userId, RoleId = 2 },
            };
            mockService.Setup(service => service.GetUserRolesByUserIdAsync(userId))
                .ReturnsAsync(expectedRoles);

            // Act
            var controller = new UserRoleController(mockService.Object);
            var result = await controller.GetRolesByUserId(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var userRoles = Assert.IsType<List<UserRoleDto>>(okResult.Value);
            Assert.Equal(expectedRoles.Count, userRoles.Count);
        }

        [Fact]
        public async Task GetUsersByRoleId_Should_ReturnOk_WhenRoleExists()
        {
            // Arrange
            var mockService = new Mock<IUserRoleService>();
            var roleId = 1;
            var expectedUsers = new List<UserRoleDto> {
                new UserRoleDto { RoleId = roleId, UserId = 1 },
                new UserRoleDto { RoleId = roleId, UserId = 2 },
            };
            mockService.Setup(service => service.GetUserRolesByRoleIdAsync(roleId))
                .ReturnsAsync(expectedUsers);

            // Act
            var controller = new UserRoleController(mockService.Object);
            var result = await controller.GetRolesByUserId(roleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var userRoles = Assert.IsType<List<UserRoleDto>>(okResult.Value);
            Assert.Equal(expectedUsers.Count, userRoles.Count);
        }

        [Fact]
        public async Task RemoveRoleFromUser_Should_ReturnOk_WhenUserRoleIsDeleted()
        {
            // Arrange
            var mockService = new Mock<IUserRoleService>();
            mockService.Setup(service => service.DeleteUserRoleAsync(1, 1))
                .Returns(Task.CompletedTask);

            // Act
            var controller = new UserRoleController(mockService.Object);
            var result = controller.RemoveRoleFromUser(1, 1);
            
            // Assert
            Assert.IsType<OkResult>(result.Result);
            mockService.Verify(service =>  service.DeleteUserRoleAsync(1, 1), Times.Once);
        }
    }
}
