using Application.DTO.Question;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Sdk;

namespace Tests
{
    public class QuestionTest
    {
        [Fact]
        public async Task GetQuestionById_Should_ReturnOk_WhenQuestionExists()
        {
            // Arrange
            var mockService = new Mock<IQuestionService>();
            int id = 1;
            mockService.Setup(service => service.GetQuestionByIdAsync(id))
                .ReturnsAsync(new QuestionDto { Id = id, Text = "Sample Question" });

            // Act
            var controller = new QuestionController(mockService.Object);
            var result = await controller.GetQuestionById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var question = Assert.IsType<QuestionDto>(okResult.Value);
            Assert.Equal(id, question.Id);
            Assert.Equal("Sample Question", question.Text);
        }

        [Fact]
        public async Task GetQuestionById_Should_ThrowNotFoundException_WhenQuestionDoesntExist()
        {
            // Arrange
            var mockService = new Mock<IQuestionService>();
            int id = 999;
            mockService.Setup(service => service.GetQuestionByIdAsync(id))
                .ThrowsAsync(new NotFoundException("Question not found"));

            // Act
            var controller = new QuestionController(mockService.Object);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => controller.GetQuestionById(id));
        }

        [Fact]
        public async Task CreateQuestion_Should_ReturnCreatedAtAction_WhenQuestionIsCreated()
        {
            // Arrange
            var mockService = new Mock<IQuestionService>();
            var createDto = new CreateQuestionDto { Text = "New Question" };
            var createdQuestionDto = new QuestionDto { Id = 1, Text = "New Question" };
            mockService.Setup(service => service.CreateQuestionAsync(createDto))
                .ReturnsAsync(createdQuestionDto);

            // Act
            var controller = new QuestionController(mockService.Object);
            var result = await controller.CreateQuestion(createDto);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var question = Assert.IsType<QuestionDto>(createdAtResult.Value);
            Assert.Equal(nameof(QuestionController.GetQuestionById), createdAtResult.ActionName);
            Assert.Equal(createdQuestionDto.Id, createdAtResult.RouteValues["id"]);
        }

        [Fact]
        public async Task GetAllQuestions_Should_ReturnOk_WithList()
        {
            // Arrange
            var mockService = new Mock<IQuestionService>();
            var expectedQuestions = new List<QuestionDto>
            {
                new QuestionDto { Id = 1, Text = "Question 1" },
                new QuestionDto { Id = 2, Text = "Question 2" }
            };
            mockService.Setup(service => service.GetAllQuestionsAsync())
                .ReturnsAsync(expectedQuestions);

            // Act
            var controller = new QuestionController(mockService.Object);
            var result = await controller.GetAllQuestions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var questions = Assert.IsType<List<QuestionDto>>(okResult.Value);
            Assert.Equal(expectedQuestions.Count, questions.Count);
        }

        [Fact]
        public async Task GetQuestionByText_Should_ReturnOk_WhenQuestionExists()
        {
            // Arrange
            var mockService = new Mock<IQuestionService>();
            string text = "Sample Question";
            mockService.Setup(service => service.GetQuestionByTextAsync(text))
                .ReturnsAsync(new QuestionDto { Id = 1, Text = text });

            // Act
            var controller = new QuestionController(mockService.Object);
            var result = await controller.GetQuestionByText(text);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var question = Assert.IsType<QuestionDto>(okResult.Value);
            Assert.Equal(text, question.Text);
        }

        [Fact]
        public async Task GetQuestionsByExamId_Should_ReturnOk_WhenQuestionsExist()
        {
            // Arrange
            var mockService = new Mock<IQuestionService>();
            int examId = 1;
            var expectedQuestions = new List<QuestionDto>
            {
                new QuestionDto { Id = 1, Text = "Question 1", ExamId = examId },
                new QuestionDto { Id = 2, Text = "Question 2", ExamId = examId }
            };
            mockService.Setup(service => service.GetQuestionsByExamIdAsync(examId))
                .ReturnsAsync(expectedQuestions);

            // Act
            var controller = new QuestionController(mockService.Object);
            var result = await controller.GetQuestionsByExamId(examId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var questions = Assert.IsType<List<QuestionDto>>(okResult.Value);
            Assert.Equal(expectedQuestions.Count, questions.Count);
        }

        [Fact]
        public async Task GetQuestionsByType_Should_ReturnOk_WhenQuestionsExist()
        {
            // Arrange
            var mockService = new Mock<IQuestionService>();
            var questionType = QuestionType.MultipleChoice;
            var expectedQuestions = new List<QuestionDto>
            {
                new QuestionDto { Id = 1, Text = "Question 1", Type = questionType },
                new QuestionDto { Id = 2, Text = "Question 2", Type = questionType }
            };
            mockService.Setup(service => service.GetQuestionsByTypeAsync(questionType))
                .ReturnsAsync(expectedQuestions);

            // Act
            var controller = new QuestionController(mockService.Object);
            var result = await controller.GetQuestionsByType(questionType);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var questions = Assert.IsType<List<QuestionDto>>(okResult.Value);
            Assert.Equal(expectedQuestions.Count, questions.Count);
        }

        [Fact]
        public async Task UpdateQuestion_Should_ReturnOk_WhenQustionIsUpdated()
        {
            // Arrange
            var mockService = new Mock<IQuestionService>();
            var updateDto = new CreateQuestionDto { Text = "Updated Question" };
            var updatedQuestion = new QuestionDto { Id = 1, Text = updateDto.Text };
            mockService.Setup(service => service.UpdateQuestionAsync(1, updateDto))
                .ReturnsAsync(updatedQuestion);

            // Act
            var controller = new QuestionController(mockService.Object);
            var result = await controller.UpdateQuestion(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var question = Assert.IsType<QuestionDto>(okResult.Value);
            Assert.Equal(updatedQuestion.Id, question.Id);
            Assert.Equal(updatedQuestion.Text, question.Text);
            mockService.Verify(service => service.UpdateQuestionAsync(1, updateDto), Times.Once);
        }

        [Fact]
        public async Task DeleteQuestion_Should_ReturnOk_WhenQuestionIsDeleted()
        {
            // Arrange
            var mockService = new Mock<IQuestionService>();
            mockService.Setup(service => service.DeleteQuestionAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            var controller = new QuestionController(mockService.Object);
            var result = await controller.DeleteQuestion(1);

            // Assert
            Assert.IsType<OkResult>(result);
            mockService.Verify(service => service.DeleteQuestionAsync(1), Times.Once);
        }
    }
}
