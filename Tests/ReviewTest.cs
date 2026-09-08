using Application.DTO.Review;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using Microsoft.AspNetCore.Builder;
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
    public class ReviewTest
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
        public async Task GetReviewById_Should_ReturnOk_WhenReviewExists()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            var id = 1;
            mockService.Setup(s => s.GetReviewByIdAsync(id))
                .ReturnsAsync(new ReviewDto { Id = id });

            // Act
            var controller = new ReviewController(mockService.Object);
            var result = await controller.GetReviewById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var review = Assert.IsType<ReviewDto>(okResult.Value);
            Assert.Equal(id, review.Id);
        }

        [Fact]
        public async Task GetReviewById_Should_ThrowNotFound_WhenReviewDoesntExists()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            var id = 1;
            mockService.Setup(s => s.GetReviewByIdAsync(id))
                .ThrowsAsync(new NotFoundException("Review not found"));

            // Act
            var controller = new ReviewController(mockService.Object);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => controller.GetReviewById(id));
        }

        [Fact]
        public async Task CreateReview_Should_ReturnCreatedAtAction_WhenReviewIsCreated()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            var userId = 1;
            var createReviewDto = new CreateReviewDto { CourseId = 1, NumOfStars = 3, Text = "Test" };
            mockService.Setup(s => s.CreateReviewAsync(createReviewDto, userId))
                .ReturnsAsync(new ReviewDto { Id = 1, CourseId = 1, UserId = 1, Text = "Test" });

            // Act
            var controller = new ReviewController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: userId) }
            };
            var result = await controller.CreateReview(createReviewDto);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var review = Assert.IsType<ReviewDto>(createdAtResult.Value);
            Assert.Equal(nameof(ReviewController.GetReviewById), createdAtResult.ActionName);
            Assert.Equal(1, createdAtResult.RouteValues["id"]);
        }

        [Fact]
        public async Task GetAllReviews_Should_ReturnOk_WithReviews()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            var userId = 1;
            var expectedReviews = new List<ReviewDto>
            {
                new ReviewDto { Id = 1, Text = "Review 1" },
                new ReviewDto { Id = 2, Text = "Review 2" },
                new ReviewDto { Id = 3 , Text = "Review 3" }
            };
            mockService.Setup(s => s.GetAllReviewsAsync())
                .ReturnsAsync(expectedReviews);

            // Act
            var controller = new ReviewController(mockService.Object);
            var result = await controller.GetAllReviews();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var reviews = Assert.IsType<List<ReviewDto>>(okResult.Value);
            Assert.Equal(expectedReviews.Count, reviews.Count);
        }

        [Fact]
        public async Task GetReviewsByCourseId_Should_ReturnOk_WithReviews()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            var courseId = 1;
            var expectedReviews = new List<ReviewDto>
            {
                new ReviewDto { Id = 1, Text = "Review 1", CourseId = courseId },
                new ReviewDto { Id = 2, Text = "Review 2", CourseId = courseId },
                new ReviewDto { Id = 3 , Text = "Review 3", CourseId = courseId }
            };
            mockService.Setup(s => s.GetReviewsByCourseIdAsync(courseId))
                .ReturnsAsync(expectedReviews);

            // Act
            var controller = new ReviewController(mockService.Object);
            var result = await controller.GetReviewsByCourseId(courseId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var reviews = Assert.IsType<List<ReviewDto>>(okResult.Value);
            Assert.Equal(expectedReviews.Count, reviews.Count);
        }

        [Fact]
        public async Task GetReviewsByUserId_Should_ReturnOk_WithReviews()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            var userId = 1;
            var expectedReviews = new List<ReviewDto>
            {
                new ReviewDto { Id = 1, Text = "Review 1", UserId = userId },
                new ReviewDto { Id = 2, Text = "Review 2", UserId = userId },
                new ReviewDto { Id = 3 , Text = "Review 3", UserId = userId }
            };
            mockService.Setup(s => s.GetReviewsByUserIdAsync(userId))
                .ReturnsAsync(expectedReviews);

            // Act
            var controller = new ReviewController(mockService.Object);
            var result = await controller.GetReviewsByUserId(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var reviews = Assert.IsType<List<ReviewDto>>(okResult.Value);
            Assert.Equal(expectedReviews.Count, reviews.Count);
        }

        [Fact]
        public async Task UpdateReview_Should_ReturnOk_WhenUserIsOwner()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            var userId = 5;
            var updateReviewDto = new UpdateReviewDto { NumOfStars = 4, Text = "Updated review" };
            mockService.Setup(s => s.GetReviewByIdAsync(1))
                .ReturnsAsync(new ReviewDto { Id = 1, UserId = userId });
            mockService.Setup(s => s.UpdateReviewAsync(1, updateReviewDto))
                .ReturnsAsync(new ReviewDto 
                { 
                    Id = 1, 
                    Text = "Updated review", 
                    NumOfStars = 4, 
                    UserId = userId 
                });

            // Act
            var controller = new ReviewController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: userId) }
            };
            var result = await controller.UpdateReview(1, updateReviewDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var review = Assert.IsType<ReviewDto>(okResult.Value);
            mockService.Verify(s => s.UpdateReviewAsync(1,updateReviewDto), Times.Once);
        }

        [Fact]
        public async Task UpdateReview_Should_ReturnForbid_WhenUserIsNotOwner()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            var userId = 5;
            var updateReviewDto = new UpdateReviewDto { NumOfStars = 4, Text = "Updated review" };
            mockService.Setup(s => s.GetReviewByIdAsync(1))
                .ReturnsAsync(new ReviewDto { Id = 1, UserId = userId });
            mockService.Setup(s => s.UpdateReviewAsync(1, updateReviewDto))
                .ReturnsAsync(new ReviewDto
                {
                    Id = 1,
                    Text = "Updated review",
                    NumOfStars = 4,
                    UserId = userId
                });

            // Act
            var controller = new ReviewController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 2) }
            };
            var result = await controller.UpdateReview(1, updateReviewDto);

            // Assert
            Assert.IsType<ForbidResult>(result.Result);
            mockService.Verify(s => s.UpdateReviewAsync(1, updateReviewDto), Times.Once);
        }

        [Fact]
        public async Task DeleteReview_Should_ReturnOk_WhenUserIsOwner()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            var userId = 5;
            var reviewId = 1;
            mockService.Setup(s => s.GetReviewByIdAsync(reviewId))
                .ReturnsAsync(new ReviewDto { Id = reviewId, UserId = userId });
            mockService.Setup(s => s.DeleteReviewAsync(reviewId))
                .Returns(Task.CompletedTask);

            // Act
            var controller = new ReviewController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: userId) }
            };
            var result = await controller.DeleteReview(reviewId);

            // Assert
            Assert.IsType<OkResult>(result);
            mockService.Verify(s => s.DeleteReviewAsync(reviewId), Times.Once());
        }

        [Fact]
        public async Task DeleteReview_Should_ReturnOk_WhenUserIsAdmin()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            var userId = 5;
            var reviewId = 1;
            mockService.Setup(s => s.GetReviewByIdAsync(reviewId))
                .ReturnsAsync(new ReviewDto { Id = reviewId, UserId = userId });
            mockService.Setup(s => s.DeleteReviewAsync(reviewId))
                .Returns(Task.CompletedTask);

            // Act
            var controller = new ReviewController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 10, role: "Admin") }
            };
            var result = await controller.DeleteReview(reviewId);

            // Assert
            Assert.IsType<OkResult>(result);
            mockService.Verify(s => s.DeleteReviewAsync(reviewId), Times.Once());
        }

        [Fact]
        public async Task DeleteReview_Should_ReturnForbid_WhenUserIsNotOwnerOrAdmin()
        {
            // Arrange
            var mockService = new Mock<IReviewService>();
            var userId = 5;
            var reviewId = 1;
            mockService.Setup(s => s.GetReviewByIdAsync(reviewId))
                .ReturnsAsync(new ReviewDto { Id = reviewId, UserId = userId });
            mockService.Setup(s => s.DeleteReviewAsync(reviewId))
                .Returns(Task.CompletedTask);

            // Act
            var controller = new ReviewController(mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser(userId: 10) }
            };
            var result = await controller.DeleteReview(reviewId);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }
    }
}
