using Application.DTO.ExamAttempt;
using Application.DTO.ExamQuestionAnswer;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Tests
{
    public class ExamQuestionAnswerTest
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
        public async Task GetExamQuestionAnswerById_Should_ReturnOk_WhenEQA_Exists()
        {
            // Arrange
            var mockService = new Mock<IExamQuestionAnswerService>();
            var mockEA = new Mock<IExamAttemptService>();
            var questionId = 1;
            var answerId = 1;
            var attemptId = 1;
            mockService.Setup(s => s.GetExamQuestionAnswerByIdAsync(questionId, answerId, attemptId))
                .ReturnsAsync(new ExamQuestionAnswerDto
                {
                    QuestionId = questionId,
                    AnswerId = answerId,
                    AttemptId = attemptId
                });

            // Act
            var controller = new ExamQuestionAnswerController(mockService.Object, mockEA.Object);
            var result = await controller.GetExamQuestionAnswerById(questionId, answerId, attemptId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var eqa = Assert.IsType<ExamQuestionAnswerDto>(okResult.Value);
            Assert.Equal(questionId, eqa.QuestionId);
            Assert.Equal(answerId, eqa.AnswerId);
            Assert.Equal(attemptId, eqa.AttemptId);
        }

        [Fact]
        public async Task GetExamQuestionAnswerById_Should_ThrowNotFoundException_WhenEQA_DoesntExists()
        {
            // Arrange
            var mockService = new Mock<IExamQuestionAnswerService>();
            var mockEA = new Mock<IExamAttemptService>();
            var questionId = 999;
            var answerId = 888;
            var attemptId = 777;
            mockService.Setup(s => s.GetExamQuestionAnswerByIdAsync(questionId, answerId, attemptId))
                .ThrowsAsync(new NotFoundException("EQA not found"));

            // Act
            var controller = new ExamQuestionAnswerController(mockService.Object, mockEA.Object);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() 
                => controller.GetExamQuestionAnswerById(questionId, answerId, attemptId));
        }

        [Fact]
        public async Task CreateExamQuestionAnswer_Should_ReturnCreatedAtAction_WhenUserIsOwner()
        {
            // Arrange
            var mockService = new Mock<IExamQuestionAnswerService>();
            var mockEA = new Mock<IExamAttemptService>();
            var createEQA = new CreateExamQuestionAnswerDto
            {
                QuestionId = 1,
                AnswerId = 2,
                AttemptId = 3
            };
            mockService.Setup(s => s.CreateExamQuestionAnswerAsync(createEQA))
                .ReturnsAsync(new ExamQuestionAnswerDto { QuestionId = 1, AnswerId = 2, AttemptId = 3 });
            mockEA.Setup(s => s.GetExamAttemptByIdAsync(3))
                .ReturnsAsync(new ExamAttemptDto { Id = 3, UserId = 5 });

            // Act
            var controller = new ExamQuestionAnswerController(mockService.Object, mockEA.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 5) }
            };
            var result = await controller.CreateExamQuestionAnswer(createEQA);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var eqa = Assert.IsType<ExamQuestionAnswerDto>(createdAtResult.Value);
            Assert.Equal(nameof(ExamQuestionAnswerController.GetExamQuestionAnswerById), createdAtResult.ActionName);
            Assert.Equal(1, createdAtResult.RouteValues["questionId"]);
            Assert.Equal(2, createdAtResult.RouteValues["answerId"]);
            Assert.Equal(3, createdAtResult.RouteValues["attemptId"]);
        }

        [Fact]
        public async Task CreateExamQuestionAnswer_Should_ReturnForbid_WhenUserIsNotOwner()
        {
            // Arrange
            var mockService = new Mock<IExamQuestionAnswerService>();
            var mockEA = new Mock<IExamAttemptService>();
            var createEQA = new CreateExamQuestionAnswerDto
            {
                QuestionId = 1,
                AnswerId = 2,
                AttemptId = 3
            };
            mockService.Setup(s => s.CreateExamQuestionAnswerAsync(createEQA))
                .ReturnsAsync(new ExamQuestionAnswerDto { QuestionId = 1, AnswerId = 2, AttemptId = 3 });
            mockEA.Setup(s => s.GetExamAttemptByIdAsync(3))
                .ReturnsAsync(new ExamAttemptDto { Id = 3, UserId = 5 });

            // Act
            var controller = new ExamQuestionAnswerController(mockService.Object, mockEA.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 3) }
            };
            var result = await controller.CreateExamQuestionAnswer(createEQA);

            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task GetAllExamQuestionAnswers_Should_ReturnOk_WithExamQuestionAnswers()
        {
            // Arrange
            var mockService = new Mock<IExamQuestionAnswerService>();
            var mockEA = new Mock<IExamAttemptService>();
            var expectedEqas = new List<ExamQuestionAnswerDto>
            {
                new ExamQuestionAnswerDto{ AnswerId = 1, AttemptId = 1, QuestionId = 1 },
                new ExamQuestionAnswerDto{ AnswerId = 2, AttemptId = 2, QuestionId = 2 },
                new ExamQuestionAnswerDto{ AnswerId = 3, AttemptId = 3, QuestionId = 3 }
            };
            mockService.Setup(s => s.GetAllExamQuestionAnswersAsync())
                .ReturnsAsync(expectedEqas);

            // Act
            var controller = new ExamQuestionAnswerController(mockService.Object, mockEA.Object);
            var result = await controller.GetAllExamQuestionAnswers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var eqas = Assert.IsType<List<ExamQuestionAnswerDto>>(okResult.Value);
            Assert.Equal(expectedEqas.Count, eqas.Count);
        }

        [Fact]
        public async Task GetExamQuestionAnswersByAnswerId_Should_ReturnOk_WithEQAs()
        {
            // Arrange
            var mockService = new Mock<IExamQuestionAnswerService>();
            var mockEA = new Mock<IExamAttemptService>();
            var answerId = 1;
            var expectedEqas = new List<ExamQuestionAnswerDto>
            {
                new ExamQuestionAnswerDto{ AnswerId = answerId, AttemptId = 1, QuestionId = 1 },
                new ExamQuestionAnswerDto{ AnswerId = answerId, AttemptId = 2, QuestionId = 2 },
                new ExamQuestionAnswerDto{ AnswerId = answerId, AttemptId = 3, QuestionId = 3 }
            };
            mockService.Setup(s => s.GetExamQuestionAnswerByAnswerIdAsync(answerId))
                .ReturnsAsync(expectedEqas);

            // Act
            var controller = new ExamQuestionAnswerController(mockService.Object, mockEA.Object);
            var result = await controller.GetExamQuestionAnswersByAnswerId(answerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var eqas = Assert.IsType<List<ExamQuestionAnswerDto>>(okResult.Value);
            Assert.Equal(expectedEqas.Count, eqas.Count);
        }

        [Fact]
        public async Task GetExamQuestionAnswersByQuestionId_Should_ReturnOk_WithEQAs()
        {
            // Arrange
            var mockService = new Mock<IExamQuestionAnswerService>();
            var mockEA = new Mock<IExamAttemptService>();
            var questionId = 1;
            var expectedEqas = new List<ExamQuestionAnswerDto>
            {
                new ExamQuestionAnswerDto{ AnswerId = 1, AttemptId = 1, QuestionId = questionId },
                new ExamQuestionAnswerDto{ AnswerId = 2, AttemptId = 2, QuestionId = questionId },
                new ExamQuestionAnswerDto{ AnswerId = 3, AttemptId = 3, QuestionId = questionId }
            };
            mockService.Setup(s => s.GetExamQuestionAnswerByQuestionIdAsync(questionId))
                .ReturnsAsync(expectedEqas);

            // Act
            var controller = new ExamQuestionAnswerController(mockService.Object, mockEA.Object);
            var result = await controller.GetExamQuestionAnswersByQuestionId(questionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var eqas = Assert.IsType<List<ExamQuestionAnswerDto>>(okResult.Value);
            Assert.Equal(expectedEqas.Count, eqas.Count);
        }

        [Fact]
        public async Task GetExamQuestionAnswersByAttemptId_Should_ReturnOk_WithEQAs()
        {
            // Arrange
            var mockService = new Mock<IExamQuestionAnswerService>();
            var mockEA = new Mock<IExamAttemptService>();
            var attemptId = 1;
            var expectedEqas = new List<ExamQuestionAnswerDto>
            {
                new ExamQuestionAnswerDto{ AnswerId = 1, AttemptId = attemptId, QuestionId = 1 },
                new ExamQuestionAnswerDto{ AnswerId = 2, AttemptId = attemptId, QuestionId = 2 },
                new ExamQuestionAnswerDto{ AnswerId = 3, AttemptId = attemptId, QuestionId = 3 }
            };
            mockService.Setup(s => s.GetExamQuestionAnswerByAttemptIdAsync(attemptId))
                .ReturnsAsync(expectedEqas);

            // Act
            var controller = new ExamQuestionAnswerController(mockService.Object, mockEA.Object);
            var result = await controller.GetExamQuestionAnswersByAttemptId(attemptId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var eqas = Assert.IsType<List<ExamQuestionAnswerDto>>(okResult.Value);
            Assert.Equal(expectedEqas.Count, eqas.Count);
        }

        [Fact]
        public async Task DeleteExamQuestionAnswer_Should_ReturnOk_WhenEQA_Is_Deleted()
        {
            // Arrange
            var mockService = new Mock<IExamQuestionAnswerService>();
            var mockEA = new Mock<IExamAttemptService>();
            mockService.Setup(s => s.DeleteExamQuestionAnswerAsync(1, 1, 1))
                .Returns(Task.CompletedTask);

            // Act
            var controller = new ExamQuestionAnswerController(mockService.Object, mockEA.Object);
            var result = await controller.DeleteExamQuestionAnswer(1, 1, 1);

            // Assert
            Assert.IsType<OkResult>(result);
        }
    }
}
