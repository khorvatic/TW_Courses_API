using Application.DTO.ExamAttempt;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Moq;
using Presentation.Controllers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Tests
{
    public class ExamAttemptTest
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
        public async Task GetExamAttemptById_Should_ReturnOk_WhenUserIsOwner()
        {
            var mockService = new Mock<IExamAttemptService>();
            var id = 3;
            var userId = 1;
            mockService.Setup(s => s.GetExamAttemptByIdAsync(id))
                .ReturnsAsync(new ExamAttemptDto { Id = id, ExamId = 10, UserId = userId });

            // Act
            var controller = new ExamAttemptController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: userId) }
            };
            var result = await controller.GetExamAttemptById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var examAttempt = Assert.IsType<ExamAttemptDto>(okResult.Value);
            Assert.Equal(id, examAttempt.Id);
            Assert.Equal(10, examAttempt.ExamId);
        }

        [Fact]
        public async Task GetExamAttemptById_Should_ReturnOk_WhenUserIsAdmin()
        {
            var mockService = new Mock<IExamAttemptService>();
            var id = 3;
            var userId = 1;
            mockService.Setup(s => s.GetExamAttemptByIdAsync(id))
                .ReturnsAsync(new ExamAttemptDto { Id = id, ExamId = 10, UserId = userId });

            // Act
            var controller = new ExamAttemptController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 3, role: "Admin") }
            };
            var result = await controller.GetExamAttemptById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var examAttempt = Assert.IsType<ExamAttemptDto>(okResult.Value);
            Assert.Equal(id, examAttempt.Id);
            Assert.Equal(10, examAttempt.ExamId);
        }

        [Fact]
        public async Task GetExamAttemptById_Should_ReturnOk_WhenUserIsInstructor()
        {
            var mockService = new Mock<IExamAttemptService>();
            var id = 3;
            var userId = 1;
            mockService.Setup(s => s.GetExamAttemptByIdAsync(id))
                .ReturnsAsync(new ExamAttemptDto { Id = id, ExamId = 10, UserId = userId });

            // Act
            var controller = new ExamAttemptController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 3, role: "Instructor") }
            };
            var result = await controller.GetExamAttemptById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var examAttempt = Assert.IsType<ExamAttemptDto>(okResult.Value);
            Assert.Equal(id, examAttempt.Id);
            Assert.Equal(10, examAttempt.ExamId);
        }

        [Fact]
        public async Task GetExamAttemptById_Should_ReturnForbid_WhenUserIsNotOwnerAdminInstructor()
        {
            var mockService = new Mock<IExamAttemptService>();
            var id = 3;
            var userId = 1;
            mockService.Setup(s => s.GetExamAttemptByIdAsync(id))
                .ReturnsAsync(new ExamAttemptDto { Id = id, ExamId = 10, UserId = userId });

            // Act
            var controller = new ExamAttemptController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 3) }
            };
            var result = await controller.GetExamAttemptById(id);

            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task GetExamAttemptById_Should_ThrowNotFoundException_WhenExamAttemptDoesntExist()
        {
            var mockService = new Mock<IExamAttemptService>();
            var id = 3;
            var userId = 1;
            mockService.Setup(s => s.GetExamAttemptByIdAsync(id))
                .ThrowsAsync(new NotFoundException("Exam attempt not found"));

            // Act
            var controller = new ExamAttemptController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: userId) }
            };

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => controller.GetExamAttemptById(id));
        }

        [Fact]
        public async Task CreateExamAttempt_Should_ReturnCreatedAtAction_WhenUserIsOwner()
        {
            // Arrange
            var mockService = new Mock<IExamAttemptService>();
            var userId = 5;
            var createExamAttemptDto = new CreateExamAttemptDto { ExamId = 1, UserId = userId };
            mockService.Setup(s => s.CreateExamAttemptAsync(createExamAttemptDto))
                .ReturnsAsync(new ExamAttemptDto { Id = 1, ExamId = 1, UserId = userId });

            // Act
            var controller = new ExamAttemptController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: userId) }
            };
            var result = await controller.CreateExamAttempt(createExamAttemptDto);

            // Assert
            var createdAtAction = Assert.IsType<CreatedAtActionResult>(result.Result);
            var examAttempt = Assert.IsType<ExamAttemptDto>(createdAtAction.Value);
            Assert.Equal(nameof(ExamAttemptController.GetExamAttemptById), createdAtAction.ActionName);
            Assert.Equal(1, createdAtAction.RouteValues["id"]);
        }

        [Fact]
        public async Task CreateExamAttempt_Should_ReturnForbid_WhenUserIsNotOwner()
        {
            // Arrange
            var mockService = new Mock<IExamAttemptService>();
            var userId = 5;
            var createExamAttemptDto = new CreateExamAttemptDto { ExamId = 1, UserId = userId };
            mockService.Setup(s => s.CreateExamAttemptAsync(createExamAttemptDto))
                .ReturnsAsync(new ExamAttemptDto { Id = 1, ExamId = 1, UserId = userId });

            // Act
            var controller = new ExamAttemptController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 10) }
            };
            var result = await controller.CreateExamAttempt(createExamAttemptDto);

            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task GetAllExamAttempts_Should_ReturnOk_WithExamAttempts()
        {
            // Arrange
            var mockService = new Mock<IExamAttemptService>();
            var expectedExamAttempts = new List<ExamAttemptDto>
            {
                new ExamAttemptDto {Id = 1, ExamId = 1, UserId = 1 },
                new ExamAttemptDto {Id = 2, ExamId =1, UserId = 2 },
            };
            mockService.Setup(s => s.GetAllExamAttemptsAsync())
                .ReturnsAsync(expectedExamAttempts);

            // Act
            var controller = new ExamAttemptController(mockService.Object);
            var result = await controller.GetAllExamAttempts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var examAttempts = Assert.IsType<List<ExamAttemptDto>>(okResult.Value);
            Assert.Equal(expectedExamAttempts.Count, examAttempts.Count);
        }

        [Fact]
        public async Task GetExamAttemptsByExamId_Should_ReturnOk_WithExamAttempts()
        {
            // Arrange
            var mockService = new Mock<IExamAttemptService>();
            var examId = 1;
            var expectedExamAttempts = new List<ExamAttemptDto>
            {
                new ExamAttemptDto { Id = 1, ExamId = examId, UserId = 2, Passed = true, Score = 60 },
                new ExamAttemptDto { Id =2, ExamId = examId, UserId = 5, Passed = false, Score = 35 }
            };
            mockService.Setup(s => s.GetExamAttemptsByExamIdAsync(examId))
                .ReturnsAsync(expectedExamAttempts);

            // Act
            var controller = new ExamAttemptController(mockService.Object);
            var result = await controller.GetExamAttemptsByExamId(examId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var examAttempts = Assert.IsType<List<ExamAttemptDto>>(okResult.Value);
            Assert.Equal(expectedExamAttempts.Count, examAttempts.Count);
        }

        [Fact]
        public async Task GetExamAttemptsByUserId_Should_ReturnOk_WhenUserIsOwner()
        {
            // Arrange
            var mockService = new Mock<IExamAttemptService>();
            var userId = 1;
            var expectedExamAttempts = new List<ExamAttemptDto>
            {
                new ExamAttemptDto { Id = 1, ExamId = 1, UserId = userId, Passed = true, Score = 60 },
                new ExamAttemptDto { Id =2, ExamId = 2, UserId = userId, Passed = false, Score = 35 }
            };
            mockService.Setup(s => s.GetExamAttemptsByUserIdAsync(userId))
                .ReturnsAsync(expectedExamAttempts);

            // Act
            var controller = new ExamAttemptController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 1) }
            };
            var result = await controller.GetExamAttemptsByUserId(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var examAttempts = Assert.IsType<List<ExamAttemptDto>>(okResult.Value);
            Assert.Equal(expectedExamAttempts.Count, examAttempts.Count);
        }

        [Fact]
        public async Task GetExamAttemptsByUserId_Should_ReturnOk_WhenUserIsAdmin()
        {
            // Arrange
            var mockService = new Mock<IExamAttemptService>();
            var userId = 1;
            var expectedExamAttempts = new List<ExamAttemptDto>
            {
                new ExamAttemptDto { Id = 1, ExamId = 1, UserId = userId, Passed = true, Score = 60 },
                new ExamAttemptDto { Id =2, ExamId = 2, UserId = userId, Passed = false, Score = 35 }
            };
            mockService.Setup(s => s.GetExamAttemptsByUserIdAsync(userId))
                .ReturnsAsync(expectedExamAttempts);

            // Act
            var controller = new ExamAttemptController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 10, role: "Admin") }
            };
            var result = await controller.GetExamAttemptsByUserId(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var examAttempts = Assert.IsType<List<ExamAttemptDto>>(okResult.Value);
            Assert.Equal(expectedExamAttempts.Count, examAttempts.Count);
        }

        [Fact]
        public async Task GetExamAttemptsByUserId_Should_ReturnOk_WhenUserIsInstructor()
        {
            // Arrange
            var mockService = new Mock<IExamAttemptService>();
            var userId = 1;
            var expectedExamAttempts = new List<ExamAttemptDto>
            {
                new ExamAttemptDto { Id = 1, ExamId = 1, UserId = userId, Passed = true, Score = 60 },
                new ExamAttemptDto { Id =2, ExamId = 2, UserId = userId, Passed = false, Score = 35 }
            };
            mockService.Setup(s => s.GetExamAttemptsByUserIdAsync(userId))
                .ReturnsAsync(expectedExamAttempts);

            // Act
            var controller = new ExamAttemptController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 10, role: "Instructor") }
            };
            var result = await controller.GetExamAttemptsByUserId(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var examAttempts = Assert.IsType<List<ExamAttemptDto>>(okResult.Value);
            Assert.Equal(expectedExamAttempts.Count, examAttempts.Count);
        }

        [Fact]
        public async Task GetExamAttemptsByUserId_Should_ReturnForbid_WhenUserIsNotOwnerInstructorAdmin()
        {
            // Arrange
            var mockService = new Mock<IExamAttemptService>();
            var userId = 1;
            var expectedExamAttempts = new List<ExamAttemptDto>
            {
                new ExamAttemptDto { Id = 1, ExamId = 1, UserId = userId, Passed = true, Score = 60 },
                new ExamAttemptDto { Id =2, ExamId = 2, UserId = userId, Passed = false, Score = 35 }
            };
            mockService.Setup(s => s.GetExamAttemptsByUserIdAsync(userId))
                .ReturnsAsync(expectedExamAttempts);

            // Act
            var controller = new ExamAttemptController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 5) }
            };
            var result = await controller.GetExamAttemptsByUserId(userId);

            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task SubmitExam_Should_ReturnOk_WhenUserIsOwner()
        {
            // Arrange
            var mockService = new Mock<IExamAttemptService>();
            var attemptId = 1;
            var userId = 5;
            mockService.Setup(s => s.GetExamAttemptByIdAsync(attemptId))
                .ReturnsAsync(new ExamAttemptDto
                {
                    Id = attemptId,
                    UserId = userId,
                    ExamId = 3
                });
            mockService.Setup(s => s.SubmitExamAttemptAsync(attemptId))
                .ReturnsAsync(new ExamAttemptDto
                {
                    Id = attemptId,
                    ExamId = 3,
                    UserId = userId,
                    Passed = false,
                    Score = 30
                });

            // Act
            var controller = new ExamAttemptController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: userId) }
            };
            var result = await controller.SubmitExam(attemptId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var examAttempt = Assert.IsType<ExamAttemptDto>(okResult.Value);
            Assert.False(examAttempt.Passed);
            Assert.Equal(30, examAttempt.Score);
            mockService.Verify(s => s.SubmitExamAttemptAsync(attemptId), Times.Once);
        }

        [Fact]
        public async Task SubmitExam_Should_ReturnForbid_WhenUserIsNotOwner()
        {
            // Arrange
            var mockService = new Mock<IExamAttemptService>();
            var attemptId = 1;
            var userId = 5;
            mockService.Setup(s => s.GetExamAttemptByIdAsync(attemptId))
                .ReturnsAsync(new ExamAttemptDto
                {
                    Id = attemptId,
                    UserId = userId,
                    ExamId = 3
                });
            mockService.Setup(s => s.SubmitExamAttemptAsync(attemptId))
                .ReturnsAsync(new ExamAttemptDto
                {
                    Id = attemptId,
                    ExamId = 3,
                    UserId = userId,
                    Passed = false,
                    Score = 30
                });

            // Act
            var controller = new ExamAttemptController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 10) }
            };
            var result = await controller.SubmitExam(attemptId);

            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }
    }
}
