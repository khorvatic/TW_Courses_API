using Application.DTO.Login;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    public class AuthTest
    {
        [Fact]
        public async Task Login_ReturnsOk_WhenCredentialsAreValid()
        {
            // Arrange
            var mockService = new Mock<IAuthService>();
            var loginDto = new LoginDto { Email = "testuser@mail.com", Password = "password" };
            var expectedToken = "mocked-jwt-token";
            mockService.Setup(service => service.LoginAsync(loginDto))
                .ReturnsAsync(expectedToken);

            // Act
            var controller = new AuthController(mockService.Object);
            var result = await controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var token = Assert.IsType<string>(okResult.Value);
            Assert.Equal(expectedToken, token);
        }

        [Fact]
        public async Task Login_ThrowsUnauthorized_WhenCredentialsArentValid()
        {
            // Arrange
            var mockService = new Mock<IAuthService>();
            var loginDto = new LoginDto
            {
                Email = "test@mail.com",
                Password = "wrongpassword"
            };
            mockService.Setup(service => service.LoginAsync(It.IsAny<LoginDto>()))
                .ThrowsAsync(new UnauthorizedException("Invalid credentials"));

            // Act
            var controller = new AuthController(mockService.Object);

            // Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => controller.Login(loginDto));
        }
    }
}
