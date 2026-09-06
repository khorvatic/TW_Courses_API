using Application.DTO.Course;
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
    public class CourseTest
    {
        [Fact]
        public async Task GetAllCourses_ReturnsPredeterminedList()
        {
            // Arrange
            var mockService = new Mock<ICourseService>();

            mockService.Setup(service => service.GetAllCoursesAsync())
                .ReturnsAsync(new List<CourseDto>
                {
                    new CourseDto { Id = 1, Name = "Course 1" },
                    new CourseDto { Id = 2, Name = "Course 2" }
                });

            // Act
            var controller = new CourseController(mockService.Object);
            var result = await controller.GetAllCourses();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var courses = Assert.IsType<List<CourseDto>>(okResult.Value);
            Assert.Equal(2, courses.Count);
        }

        [Fact]
        public async Task GetCourseById_ReturnsOk_WhenCourseExists()
        {
            var mockService = new Mock<ICourseService>();

            mockService.Setup(service => service.GetCourseByIdAsync(1))
                .ReturnsAsync(new CourseDto { Id = 1, Name = "Test" });

            var controller = new CourseController(mockService.Object);
            var result = await controller.GetCourseById(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var course = Assert.IsType<CourseDto>(okResult.Value);
            Assert.Equal("Test", course.Name);
        }

        [Fact]
        public async Task GetCourseById_ThrowsNotFound_WhenCourseDoesntExist()
        {
            var mockService = new Mock<ICourseService>();
            mockService.Setup(service => service.GetCourseByIdAsync(999))
                .ThrowsAsync(new NotFoundException("Course not found"));

            var controller = new CourseController(mockService.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => controller.GetCourseById(999));
        }

        [Fact]
        public async Task CreateCourse_Should_ReturnCreatedAtAction_WhenCourseIsCreated()
        {
            // Arrange
            var mockService = new Mock<ICourseService>();
            var createCourseDto = new CreateCourseDto { Name = "New Course" };
            var createdCourseDto = new CourseDto { Id = 1, Name = "New Course" };

            mockService.Setup(c => c.CreateCourseAsync(createCourseDto))
                .ReturnsAsync(createdCourseDto);

            // Act
            var controller = new CourseController(mockService.Object);
            var result = await controller.CreateCourse(createCourseDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedCourse = Assert.IsType<CourseDto>(createdAtActionResult.Value);
            Assert.Equal(nameof(CourseController.GetCourseById), createdAtActionResult.ActionName);
            Assert.Equal(createdCourseDto.Id, createdAtActionResult.RouteValues["id"]);
        }

        [Fact]
        public async Task GetCourseByName_Should_ReturnOk_WhenCourseExists()
        {
            // Arrange
            var mockService = new Mock<ICourseService>();
            mockService.Setup(service => service.GetCourseByNameAsync("Test"))
                .ReturnsAsync(new CourseDto { Id = 1, Name = "Test" });

            // Act
            var controller = new CourseController(mockService.Object);
            var result = await controller.GetCourseByName("Test");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var course = Assert.IsType<CourseDto>(okResult.Value);
            Assert.Equal("Test", course.Name);
        }

        [Fact]
        public async Task UpdateCourse_Should_ReturnOk_WhenCourseUpdated()
        {
            // Arrange
            var mockService = new Mock<ICourseService>();
            var updateCourseDto = new CreateCourseDto { Name = "Updated Course" };
            var updatedCourseDto = new CourseDto
            {
                Id = 1,
                Name = "Updated Course"
            };

            mockService.Setup(service => service.UpdateCourseAsync(1, updateCourseDto))
                .ReturnsAsync(updatedCourseDto);

            // Act
            var controller = new CourseController(mockService.Object);
            var result = await controller.UpdateCourse(1, updateCourseDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var updatedCourse = Assert.IsType<CourseDto>(okResult.Value);
            Assert.Equal("Updated Course", updatedCourse.Name);
            mockService.Verify(c => c.UpdateCourseAsync(1, updateCourseDto), Times.Once);
        }

        [Fact]
        public async Task DeleteCourse_Should_ReturnOk_WhenCourseDeleted()
        {
            // Arrange
            var mockService = new Mock<ICourseService>();
            mockService.Setup(service => service.DeleteCourseAsync(1))
                .Returns(Task.CompletedTask);
            
            // Act
            var controller = new CourseController(mockService.Object);
            var result = await controller.DeleteCourse(1);
            
            // Assert
            Assert.IsType<OkResult>(result);
            mockService.Verify(c => c.DeleteCourseAsync(1), Times.Once);
        }
    }
}
