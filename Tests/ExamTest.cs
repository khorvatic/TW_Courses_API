using Application.DTO.Exam;
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
    public class ExamTest
    {
        [Fact]
        public async Task GetExamById_Should_ReturnOk_WhenExamExists()
        {
            // Arrange
            var mockService = new Mock<IExamService>();
            var examId = 1;
            var expectedExam = new ExamDto { Id = examId, Title = "Sample Exam" };
            mockService.Setup(service => service.GetExamByIdAsync(examId))
                .ReturnsAsync(expectedExam);

            // Act
            var controller = new ExamController(mockService.Object);
            var result = await controller.GetExamById(examId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var exam = Assert.IsType<ExamDto>(okResult.Value);
            Assert.Equal(expectedExam.Id, exam.Id);
            Assert.Equal(expectedExam.Title, exam.Title);
        }

        [Fact]
        public async Task GetExamById_Should_ThrowNotFoundException_WhenExamDoesntExist()
        {
            // Arrange
            var mockService = new Mock<IExamService>();
            var id = 999;
            mockService.Setup(service => service.GetExamByIdAsync(id))
                .ThrowsAsync(new NotFoundException("Exam not found"));

            // Act
            var controller = new ExamController(mockService.Object);
            
            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => controller.GetExamById(id));
        }

        [Fact]
        public async Task GetAllExams_Should_ReturnOk_WithList()
        {
            // Arrange
            var mockService = new Mock<IExamService>();
            var expectedExams = new List<ExamDto>
            {
                new ExamDto { Id = 1, Title = "Exam 1" },
                new ExamDto { Id = 2, Title = "Exam 2" }
            };
            mockService.Setup(service => service.GetAllExamsAsync())
                .ReturnsAsync(expectedExams);

            // Act
            var controller = new ExamController(mockService.Object);
            var result = await controller.GetAllExams();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var exams = Assert.IsType<List<ExamDto>>(okResult.Value);
            Assert.Equal(expectedExams.Count, exams.Count);
        }

        [Fact]
        public async Task CreateExam_Should_ReturnCreatedAtAction_WhenExamIsCreated()
        {
            // Arrange
            var mockService = new Mock<IExamService>();
            var createExamDto = new CreateExamDto { Title = "New Exam" };
            var createdExam = new ExamDto { Id = 1, Title = "New Exam" };
            mockService.Setup(service => service.CreateExamAsync(createExamDto))
                .ReturnsAsync(createdExam);

            // Act
            var controller = new ExamController(mockService.Object);
            var result = await controller.CreateExam(createExamDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var exam = Assert.IsType<ExamDto>(createdAtActionResult.Value);
            Assert.Equal(nameof(ExamController.GetExamById), createdAtActionResult.ActionName);
            Assert.Equal(createdExam.Id, createdAtActionResult.RouteValues["id"]);
        }

        [Fact]
        public async Task GetExamsByCourseId_Should_ReturnOk_WithExams()
        {
            // Arrange
            var mockService = new Mock<IExamService>();
            var courseId = 1;
            var expectedExams = new List<ExamDto>
            {
                new ExamDto { Id = 1, Title = "Exam 1", CourseId = courseId },
                new ExamDto { Id = 2, Title = "Exam 2", CourseId = courseId }
            };
            mockService.Setup(service => service.GetExamsByCourseIdAsync(courseId))
                .ReturnsAsync(expectedExams);

            // Act
            var controller = new ExamController(mockService.Object);
            var result = await controller.GetExamsByCourseId(courseId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var exams = Assert.IsType<List<ExamDto>>(okResult.Value);
            Assert.Equal(expectedExams.Count, exams.Count);
        }

        [Fact]
        public async Task GetExamByTitle_Should_ReturnOk()
        {
            // Arrange
            var mockService = new Mock<IExamService>();
            var title = "Sample exam";
            var expectedExam = new ExamDto { Id = 1, Title = title };
            mockService.Setup(service => service.GetExamByTitleAsync(title))
                .ReturnsAsync(expectedExam);

            // Act
            var controller = new ExamController(mockService.Object);
            var result = await controller.GetExamByTitle(title);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var exam = Assert.IsType<ExamDto>(okResult.Value);
            Assert.Equal(expectedExam.Id, exam.Id);
            Assert.Equal(title, exam.Title);
        }

        [Fact]
        public async Task UpdateExam_Should_ReturnOk_WhenExamIsUpdated()
        {
            // Arrange
            var mockService = new Mock<IExamService>();
            var examId = 1;
            var updateExamDto = new CreateExamDto { Title = "Updated Exam" };
            var updatedExam = new ExamDto { Id = examId, Title = "Updated Exam" };
            mockService.Setup(service => service.UpdateExamAsync(examId, updateExamDto))
                .ReturnsAsync(updatedExam);

            // Act
            var controller = new ExamController(mockService.Object);
            var result = await controller.UpdateExam(examId, updateExamDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var exam = Assert.IsType<ExamDto>(okResult.Value);
            Assert.Equal(updatedExam.Id, exam.Id);
            Assert.Equal(updatedExam.Title, exam.Title);
        }

        [Fact]
        public async Task DeleteExam_Should_ReturnOk_WhenExamIsDeleted()
        {
            // Arrange
            var mockService = new Mock<IExamService>();
            var id = 1;
            mockService.Setup(service => service.RemoveExamAsync(id))
                .Returns(Task.CompletedTask);

            // Act
            var controller = new ExamController(mockService.Object);
            var result = await controller.DeleteExam(id);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
        }
    }
}
