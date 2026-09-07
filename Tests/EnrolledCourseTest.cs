using Application.DTO.EnrolledCourse;
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
    public class EnrolledCourseTest
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
        public async Task GetEnrolledCourseById_Should_ReturnOk_WhenUserIsOwner()
        {
            // Arrange
            var mockService = new Mock<IEnrolledCourseService>();
            var enrolledCourseId = 1;
            mockService.Setup(service => service.GetEnrolledCourseByIdAsync(enrolledCourseId))
                .ReturnsAsync(new EnrolledCourseDto { 
                    Id = enrolledCourseId, 
                    Completed = false, 
                    CourseId = 2, 
                    UserId = 5 
                });

            // Act
            var controller = new EnrolledCourseController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 5) }
            };
            var result = await controller.GetEnrolledCourseById(enrolledCourseId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var enrolledCourse = Assert.IsType<EnrolledCourseDto>(okResult.Value);
            Assert.Equal(enrolledCourseId, enrolledCourse.Id);
            Assert.Equal(5, enrolledCourse.UserId);
        }

        [Fact]
        public async Task GetEnrolledCourseById_Should_ReturnOk_WhenUserIsAdmin()
        {
            // Arrange
            var mockService = new Mock<IEnrolledCourseService>();
            var enrolledCourseId = 1;
            mockService.Setup(service => service.GetEnrolledCourseByIdAsync(enrolledCourseId))
                .ReturnsAsync(new EnrolledCourseDto
                {
                    Id = enrolledCourseId,
                    Completed = false,
                    CourseId = 2,
                    UserId = 5
                });

            // Act
            var controller = new EnrolledCourseController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 10, role: "Admin") }
            };
            var result = await controller.GetEnrolledCourseById(enrolledCourseId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var enrolledCourse = Assert.IsType<EnrolledCourseDto>(okResult.Value);
            Assert.Equal(enrolledCourseId, enrolledCourse.Id);
        }

        [Fact]
        public async Task GetEnrolledCourseById_Should_ReturnForbid_WhenUserIsNotOwnerOrAdmin()
        {
            // Arrange
            var mockService = new Mock<IEnrolledCourseService>();
            var enrolledCourseId = 1;
            mockService.Setup(service => service.GetEnrolledCourseByIdAsync(enrolledCourseId))
                .ReturnsAsync(new EnrolledCourseDto
                {
                    Id = enrolledCourseId,
                    Completed = false,
                    CourseId = 2,
                    UserId = 5
                });

            // Act
            var controller = new EnrolledCourseController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 10) }
            };
            var result = await controller.GetEnrolledCourseById(enrolledCourseId);

            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task GetEnrolledCourseById_Should_ThrowNotFoundException_WhenEnrolledCourseDoesntExist()
        {
            // Arrange
            var mockService = new Mock<IEnrolledCourseService>();
            mockService.Setup(service => service.GetEnrolledCourseByIdAsync(999))
                .ThrowsAsync(new NotFoundException("Enrolled course not found"));

            // Act
            var controller = new EnrolledCourseController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 10, role: "Admin") }
            };

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => controller.GetEnrolledCourseById(999));
        }

        [Fact]
        public async Task EnrollInCourse_Should_ReturnOk_WhenUserIsOwner()
        {
            // Arrange
            var mockService = new Mock<IEnrolledCourseService>();
            var createEnrolledCourseDto = new CreateEnrolledCourseDto { CourseId = 1, UserId = 5 };
            mockService.Setup(service => service.EnrollInCourseAsync(createEnrolledCourseDto))
                .ReturnsAsync(new EnrolledCourseDto { Id = 1, Completed = false, CourseId = 1, UserId = 5 });

            // Act
            var controller = new EnrolledCourseController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 5) }
            };
            var result = await controller.EnrollInCourse(createEnrolledCourseDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var enrolledCourse = Assert.IsType<EnrolledCourseDto>(okResult.Value);
            Assert.Equal(5, enrolledCourse.UserId);
        }

        [Fact]
        public async Task EnrollInCourse_Should_ReturnBadRequest_WhenUserIsNotOwner()
        {
            // Arrange
            var mockService = new Mock<IEnrolledCourseService>();
            var createEnrolledCourseDto = new CreateEnrolledCourseDto { CourseId = 1, UserId = 5 };
            mockService.Setup(service => service.EnrollInCourseAsync(createEnrolledCourseDto))
                .ReturnsAsync(new EnrolledCourseDto { Id = 1, Completed = false, CourseId = 1, UserId = 5 });

            // Act
            var controller = new EnrolledCourseController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 10) }
            };
            var result = await controller.EnrollInCourse(createEnrolledCourseDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetEnrolledCoursesByUserId_Should_ReturnOk_WhenUserIsOwner()
        {
            // Arrange
            var mockService = new Mock<IEnrolledCourseService>();
            var userId = 1;
            var expectedCourses = new List<EnrolledCourseDto>
            {
                new EnrolledCourseDto { Id = 1, Completed = false, UserId = userId, CourseId = 2 },
                new EnrolledCourseDto { Id = 2, Completed = false, UserId = userId, CourseId = 3 }
            };
            mockService.Setup(s => s.GetEnrolledCoursesForUserAsync(userId))
                .ReturnsAsync(expectedCourses);

            // Act
            var controller = new EnrolledCourseController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: userId) }
            };
            var result = await controller.GetEnrolledCoursesByUserId(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var enrolledCourses = Assert.IsType<List<EnrolledCourseDto>>(okResult.Value);
            Assert.Equal(expectedCourses.Count, enrolledCourses.Count);
        }

        [Fact]
        public async Task GetEnrolledCoursesByUserId_Should_ReturnOk_WhenUserIsAdmin()
        {
            // Arrange
            var mockService = new Mock<IEnrolledCourseService>();
            var userId = 1;
            var expectedCourses = new List<EnrolledCourseDto>
            {
                new EnrolledCourseDto { Id = 1, Completed = false, UserId = userId, CourseId = 2 },
                new EnrolledCourseDto { Id = 2, Completed = false, UserId = userId, CourseId = 3 }
            };
            mockService.Setup(s => s.GetEnrolledCoursesForUserAsync(userId))
                .ReturnsAsync(expectedCourses);

            // Act
            var controller = new EnrolledCourseController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 10, role: "Admin") }
            };
            var result = await controller.GetEnrolledCoursesByUserId(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var enrolledCourses = Assert.IsType<List<EnrolledCourseDto>>(okResult.Value);
            Assert.Equal(expectedCourses.Count, enrolledCourses.Count);
        }

        [Fact]
        public async Task GetEnrolledCoursesByUserId_Should_ReturnForbid_WhenUserIsNotOwner()
        {
            // Arrange
            var mockService = new Mock<IEnrolledCourseService>();
            var userId = 1;
            var expectedCourses = new List<EnrolledCourseDto>
            {
                new EnrolledCourseDto { Id = 1, Completed = false, UserId = userId, CourseId = 2 },
                new EnrolledCourseDto { Id = 2, Completed = false, UserId = userId, CourseId = 3 }
            };
            mockService.Setup(s => s.GetEnrolledCoursesForUserAsync(userId))
                .ReturnsAsync(expectedCourses);

            // Act
            var controller = new EnrolledCourseController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 5) }
            };
            var result = await controller.GetEnrolledCoursesByUserId(userId);

            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task CompleteEnrolledCourse_Should_ReturnOk_WhenUserIsOwner()
        {
            // Arrange
            var mockService = new Mock<IEnrolledCourseService>();
            var userId = 1;
            var courseId = 1;
            mockService.Setup(service => service.CompleteCourseAsync(userId, courseId))
                .Returns(Task.CompletedTask);

            // Act
            var controller = new EnrolledCourseController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: userId) }
            };
            var result = await controller.CompleteEnrolledCourse(userId, courseId);

            // Assert
            Assert.IsType<OkResult>(result.Result);
        }

        [Fact]
        public async Task CompleteEnrolledCourse_Should_ReturnForbid_WhenUserIsNotOwner()
        {
            // Arrange
            var mockService = new Mock<IEnrolledCourseService>();
            var userId = 1;
            var courseId = 1;
            mockService.Setup(service => service.CompleteCourseAsync(userId, courseId))
                .Returns(Task.CompletedTask);

            // Act
            var controller = new EnrolledCourseController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 3) }
            };
            var result = await controller.CompleteEnrolledCourse(userId, courseId);

            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }
    }
}
