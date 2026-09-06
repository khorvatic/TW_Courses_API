using Application.DTO.Answer;
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
    public class AnswerTest
    {
        [Fact]
        public async Task GetAnswerById_ReturnsOk_WhenAnswerExists()
        {
            // Arrange
            var mockService = new Mock<IAnswerService>();
            mockService.Setup(service => service.GetAnswerByIdAsync(1))
                .ReturnsAsync(new AnswerDto { Id = 1, Option = "Test answer", QuestionId = 1 });

            // Act
            var controller = new AnswerController(mockService.Object);
            var result = await controller.GetAnswerById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var answer = Assert.IsType<AnswerDto>(okResult.Value);
            Assert.Equal(1, answer.Id);
            Assert.Equal("Test answer", answer.Option);
            Assert.Equal(1, answer.QuestionId);
        }

        [Fact]
        public async Task GetAnswerById_ThrowsNotFound_WhenAnswerDoesntExist()
        {
            // Arrange
            var mockService = new Mock<IAnswerService>();
            mockService.Setup(service => service.GetAnswerByIdAsync(999))
                .ThrowsAsync(new NotFoundException("Answer not found"));

            // Act
            var controller = new AnswerController(mockService.Object);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => controller.GetAnswerById(999));
        }

        [Fact]
        public async Task CreateAnswer_Should_ReturnCreatedAtAction_WhenAnserCreated()
        {
            // Arrange
            var mockService = new Mock<IAnswerService>();
            var createAnswerDto = new CreateAnswerDto
            {
                Option = "New answer",
                QuestionId = 1,
                Correct = false
            };
            var createdAnswerDto = new AnswerDto
            {
                Id = 1,
                Option = "New answer",
                QuestionId = 1
            };
            mockService.Setup(s => s.CreateAnswerAsync(createAnswerDto))
                .ReturnsAsync(createdAnswerDto);

            // Act
            var controller = new AnswerController(mockService.Object);
            var result = await controller.CreateAnswer(createAnswerDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var answer = Assert.IsType<AnswerDto>(createdAtActionResult.Value);
            Assert.Equal(nameof(AnswerController.GetAnswerById), createdAtActionResult.ActionName);
            Assert.Equal(createdAnswerDto.Id, createdAtActionResult.RouteValues["id"]);
        }

        [Fact]
        public async Task GetAllAnswers_Should_ReturnOkWithList()
        {
            // Arrange
            var mockService = new Mock<IAnswerService>();
            mockService.Setup(service => service.GetAllAnswersAsync())
                .ReturnsAsync(new List<AnswerDto>
                {
                    new AnswerDto { Id = 1, Option = "Answer 1", QuestionId = 1 },
                    new AnswerDto { Id = 2, Option = "Answer 2", QuestionId = 1 }
                });

            // Act
            var controller = new AnswerController(mockService.Object);
            var result = await controller.GetAllAnswers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var answers = Assert.IsType<List<AnswerDto>>(okResult.Value);
            Assert.Equal(2, answers.Count);
        }

        [Fact]
        public async Task UpdateAnswer_Should_ReturnOk_WhenAnswerIsUpdated()
        {
            // Arrange
            var mockService = new Mock<IAnswerService>();
            var updateAnswerDto = new CreateAnswerDto
            {
                Option = "Updated answer",
                QuestionId = 1,
                Correct = true
            };
            var updatedAnswerDto = new AnswerDto
            {
                Id = 1,
                Option = "Updated answer",
                QuestionId = 1
            };
            mockService.Setup(s => s.UpdateAnswerAsync(1, updateAnswerDto))
                .ReturnsAsync(updatedAnswerDto);

            // Act
            var controller = new AnswerController(mockService.Object);
            var result = await controller.UpdateAnswer(1, updateAnswerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var answer = Assert.IsType<AnswerDto>(okResult.Value);
            Assert.Equal(updatedAnswerDto.Id, answer.Id);
            Assert.Equal(updatedAnswerDto.Option, answer.Option);
            Assert.Equal(updatedAnswerDto.QuestionId, answer.QuestionId);
            mockService.Verify(s => s.UpdateAnswerAsync(1, updateAnswerDto), Times.Once);
        }

        [Fact]
        public async Task DeleteAnswer_Should_ReturnOk_WhenAnswerIsDeleted()
        {
            // Arrange
            var mockService = new Mock<IAnswerService>();
            mockService.Setup(s => s.DeleteAnswerAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            var controller = new AnswerController(mockService.Object);
            var result = await controller.DeleteAnswer(1);

            // Assert
            Assert.IsType<OkResult>(result);
            mockService.Verify(s => s.DeleteAnswerAsync(1), Times.Once);
        }
    }
}
