using Application.DTO.User;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Moq;
using Presentation.Controllers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Tests
{
    public class UserTest
    {
        private static ClaimsPrincipal CreateUser(int userId, string role = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            if (role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        [Fact]
        public async Task GetUserById_Should_ReturnOk_WhenUserIsOwner()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var userId = 3;
            mockService.Setup(s => s.GetUserByIdAsync(userId))
                .ReturnsAsync(new UserDto { Id = userId, Email = "test@mail.com" });

            // Act
            var controller = new UserController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: userId) }
            };
            var result = await controller.GetUserById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var user = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(userId, user.Id);
            Assert.Equal("test@mail.com", user.Email);
        }

        [Fact]
        public async Task GetUserById_Should_ReturnOk_WhenUserIsAdmin()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var userId = 3;
            mockService.Setup(s => s.GetUserByIdAsync(userId))
                .ReturnsAsync(new UserDto { Id = userId, Email = "test@mail.com" });

            // Act
            var controller = new UserController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 5, role: "Admin") }
            };
            var result = await controller.GetUserById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var user = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(userId, user.Id);
            Assert.Equal("test@mail.com", user.Email);
        }

        [Fact]
        public async Task GetUserById_Should_ReturnForbid_WhenUserIsNotOwnerOrAdmin()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var userId = 3;
            mockService.Setup(s => s.GetUserByIdAsync(userId))
                .ReturnsAsync(new UserDto { Id = userId, Email = "test@mail.com" });

            // Act
            var controller = new UserController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 5) }
            };
            var result = await controller.GetUserById(userId);

            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task GetUserById_Should_ThrowNotFoundException_WhenUserDoesntExist()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var userId = 3;
            mockService.Setup(s => s.GetUserByIdAsync(userId))
                .ThrowsAsync(new NotFoundException("User not found"));

            // Act
            var controller = new UserController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: userId) }
            };

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => controller.GetUserById(userId));
        }

        [Fact]
        public async Task GetUserByEmail_Should_ReturnOk_WhenUserExists()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var email = "test@mail.com";
            mockService.Setup(s => s.GetUserByEmailAsync(email))
                .ReturnsAsync(new UserDto { Id = 1, Email = email });

            // Act
            var controller = new UserController(mockService.Object);
            var result = await controller.GetUserByEmail(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var user = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(email, user.Email);
        }

        [Fact]
        public async Task GetUserByName_Should_ReturnOk_WithUsers()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var name = "Test";
            var expectedUsers = new List<UserDto>
            {
                new UserDto{ Id = 1, Name = name },
                new UserDto { Id = 2, Name = name },
                new UserDto { Id = 3, Name = name },
            };
            mockService.Setup(s => s.GetUsersByNameAsync(name))
                .ReturnsAsync(expectedUsers);

            // Act
            var controller = new UserController(mockService.Object);
            var result = await controller.GetUsersByName(name);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var users = Assert.IsType<List<UserDto>>(okResult.Value);
            Assert.Equal(expectedUsers.Count, users.Count);
        }

        [Fact]
        public async Task GetUserBySurname_Should_ReturnOk_WithUsers()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var surname = "Test";
            var expectedUsers = new List<UserDto>
            {
                new UserDto{ Id = 1, Surname = surname },
                new UserDto { Id = 2, Surname = surname },
                new UserDto { Id = 3, Surname = surname },
            };
            mockService.Setup(s => s.GetUsersBySurnameAsync(surname))
                .ReturnsAsync(expectedUsers);

            // Act
            var controller = new UserController(mockService.Object);
            var result = await controller.GetUsersBySurname(surname);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var users = Assert.IsType<List<UserDto>>(okResult.Value);
            Assert.Equal(expectedUsers.Count, users.Count);
        }

        [Fact]
        public async Task GetAllUsers_Should_ReturnOk_WithUsers()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var expectedUsers = new List<UserDto>
            {
                new UserDto{ Id = 1, Name = "User 1" },
                new UserDto { Id = 2, Name = "User 2"},
                new UserDto { Id = 3, Name = "User 3" },
            };
            mockService.Setup(s => s.GetAllUsersAsync())
                .ReturnsAsync(expectedUsers);

            // Act
            var controller = new UserController(mockService.Object);
            var result = await controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var users = Assert.IsType<List<UserDto>>(okResult.Value);
            Assert.Equal(expectedUsers.Count, users.Count);
        }

        [Fact]
        public async Task GetUserByFullName_Should_ReturnOk_WithUsers()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var surname = "Test";
            var name = "User";
            var expectedUsers = new List<UserDto>
            {
                new UserDto{ Id = 1, Surname = surname, Name = name },
                new UserDto { Id = 2, Surname = surname, Name = name },
                new UserDto { Id = 3, Surname = surname, Name = name },
            };
            mockService.Setup(s => s.GetUsersByFullNameAsync(name, surname))
                .ReturnsAsync(expectedUsers);

            // Act
            var controller = new UserController(mockService.Object);
            var result = await controller.GetUsersByFullName(name, surname);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var users = Assert.IsType<List<UserDto>>(okResult.Value);
            Assert.Equal(expectedUsers.Count, users.Count);
        }

        [Fact]
        public async Task DeleteUser_Should_ReturnOk_WhenUserIsOwner()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var userId = 3;
            mockService.Setup(s => s.DeleteUserAsync(userId))
                .Returns(Task.CompletedTask);

            // Act
            var controller = new UserController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: userId) }
            };
            var result = await controller.DeleteUser(userId);

            // Assert
            Assert.IsType<OkResult>(result);
            mockService.Verify(s => s.DeleteUserAsync(userId), Times.Once());
        }

        [Fact]
        public async Task DeleteUser_Should_ReturnOk_WhenUserIsAdmin()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var userId = 3;
            mockService.Setup(s => s.DeleteUserAsync(userId))
                .Returns(Task.CompletedTask);

            // Act
            var controller = new UserController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 5, role: "Admin") }
            };
            var result = await controller.DeleteUser(userId);

            // Assert
            Assert.IsType<OkResult>(result);
            mockService.Verify(s => s.DeleteUserAsync(userId), Times.Once());
        }

        [Fact]
        public async Task DeleteUser_Should_ReturnForbid_WhenUserIsNotOwnerOrAdmin()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var userId = 3;
            mockService.Setup(s => s.DeleteUserAsync(userId))
                .Returns(Task.CompletedTask);

            // Act
            var controller = new UserController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 5) }
            };
            var result = await controller.DeleteUser(userId);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task UpdateUser_Should_ReturnOk_WhenUserIsOwner()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var userId = 3;
            var updateUserDto = new CreateUserDto { Email = "test@mail.com" };
            mockService.Setup(s => s.UpdateUserAsync(userId, updateUserDto))
                .ReturnsAsync(new UserDto { Email = "test@mail.com", Id = userId });

            // Act
            var controller = new UserController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: userId) }
            };
            var result = await controller.UpdateUser(userId, updateUserDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var user = Assert.IsType<UserDto>(okResult.Value);
            mockService.Verify(s => s.UpdateUserAsync(userId, updateUserDto), Times.Once());
        }

        [Fact]
        public async Task UpdateUser_Should_ReturnOk_WhenUserIsAdmin()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var userId = 3;
            var updateUserDto = new CreateUserDto { Email = "test@mail.com" };
            mockService.Setup(s => s.UpdateUserAsync(userId, updateUserDto))
                .ReturnsAsync(new UserDto { Email = "test@mail.com", Id = userId });

            // Act
            var controller = new UserController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 5, role: "Admin") }
            };
            var result = await controller.UpdateUser(userId, updateUserDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var user = Assert.IsType<UserDto>(okResult.Value);
            mockService.Verify(s => s.UpdateUserAsync(userId, updateUserDto), Times.Once());
        }

        [Fact]
        public async Task UpdateUser_Should_ReturnForbid_WhenUserIsNotOwnerOrAdmin()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var userId = 3;
            var updateUserDto = new CreateUserDto { Email = "test@mail.com" };
            mockService.Setup(s => s.UpdateUserAsync(userId, updateUserDto))
                .ReturnsAsync(new UserDto { Email = "test@mail.com", Id = userId });

            // Act
            var controller = new UserController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 5) }
            };
            var result = await controller.UpdateUser(userId, updateUserDto);

            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }
    }
}
