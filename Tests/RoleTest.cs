using Application.DTO.Role;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Moq;
using Presentation.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    public class RoleTest
    {
        [Fact]
        public async Task GetRoleById_Should_ReturnOk_WhenRoleExists()
        {
            // Arrange
            var mockService = new Mock<IRoleService>();
            var id = 1;
            mockService.Setup(service => service.GetRoleByIdAsync(1))
                .ReturnsAsync(new RoleDto { Id = id, Name = "Sample role" });

            // Act
            var controller = new RoleController(mockService.Object);
            var result = await controller.GetRoleById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var role = Assert.IsType<RoleDto>(okResult.Value);

            Assert.Equal(id, role.Id);
            Assert.Equal("Sample role", role.Name);
        }

        [Fact]
        public async Task GetRoleById_Should_ThrowNotFoundException_WhenRoleDoesntExist()
        {
            // Arrange
            var mockService = new Mock<IRoleService>();
            var id = 999;
            mockService.Setup(service => service.GetRoleByIdAsync(id))
                .ThrowsAsync(new NotFoundException("Role not found"));

            // Act
            var controller = new RoleController(mockService.Object);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => controller.GetRoleById(id));
        }

        [Fact]
        public async Task CreateRole_Should_ReturnCreatedAtAction_WhenRoleIsCreated()
        {
            // Arrange
            var mockService = new Mock<IRoleService>();
            var createRoleDto = new CreateRoleDto { Name = "Test role" };
            var createdRole = new RoleDto { Id = 1, Name = "Test role" };
            mockService.Setup(service => service.CreateRoleAsync(createRoleDto))
                .ReturnsAsync(new RoleDto { Id = 1, Name = createRoleDto.Name });

            // Act
            var controller = new RoleController(mockService.Object);
            var result = await controller.CreateRole(createRoleDto);

            // Assert
            var createdAtAction = Assert.IsType<CreatedAtActionResult>(result.Result);
            var role = Assert.IsType<RoleDto>(createdAtAction.Value);
            Assert.Equal(nameof(RoleController.GetRoleById), createdAtAction.ActionName);
            Assert.Equal(createdRole.Id, createdAtAction.RouteValues["id"]);
        }

        [Fact]
        public async Task GetAllRoles_Should_ReturnOk_WithList()
        {
            // Arrange
            var mockService = new Mock<IRoleService>();
            var expectedRoles = new List<RoleDto>
            {
                new RoleDto { Id = 1, Name = "Role 1" },
                new RoleDto { Id = 2, Name ="Role 2" }
            };
            mockService.Setup(service => service.GetAllRolesAsync())
                .ReturnsAsync(expectedRoles);

            // Act
            var controller = new RoleController(mockService.Object);
            var result = await controller.GetAllRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var roles = Assert.IsType<List<RoleDto>>(okResult.Value);
            Assert.Equal(expectedRoles.Count, roles.Count);
        }

        [Fact]
        public async Task GetRoleByName_Should_ReturnOk_WhenRoleExists()
        {
            // Arrange
            var mockService = new Mock<IRoleService>();
            var name = "Test role";
            mockService.Setup(service => service.GetRoleByNameAsync(name))
                .ReturnsAsync(new RoleDto { Id = 1, Name = name });

            // Act
            var controller = new RoleController(mockService.Object);
            var result = await controller.GetRoleByName(name);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var role = Assert.IsType<RoleDto>(okResult.Value);
            Assert.Equal(name, role.Name);
        }

        [Fact]
        public async Task DeleteRole_Should_ReturnOk_WhenRoleIsDeleted()
        {
            // Arrange
            var mockService = new Mock<IRoleService>();
            mockService.Setup(service => service.DeleteRoleByIdAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            var controller = new RoleController(mockService.Object);
            var result = await controller.DeleteRole(1);

            // Assert
            Assert.IsType<OkResult>(result);
            mockService.Verify(service => service.DeleteRoleByIdAsync(1), Times.Once());
        }
    }
}
